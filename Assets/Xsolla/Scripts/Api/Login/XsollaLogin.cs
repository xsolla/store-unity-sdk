using System;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Text;
using Xsolla.Core;

namespace Xsolla.Login
{
	public class XsollaLogin : MonoSingleton<XsollaLogin>
	{
		[SerializeField]
		string loginId;
		[SerializeField]
		bool isJwtValidationToken;
		[SerializeField]
		string jwtValidationUrl;
		[SerializeField]
		bool isProxy;
		[SerializeField]
		string callbackUrl;
		
		public string LoginID
		{
			get { return loginId; }
			set { loginId = value; }
		}
		
		public bool IsJWTValidationToken
		{
			get { return isJwtValidationToken; }
			set { isJwtValidationToken = value; }
		}

		public string JWTValidationURL
		{
			get { return jwtValidationUrl; }
			set { jwtValidationUrl = value; }
		}
		
		public bool IsProxy
		{
			get { return isProxy; }
			set { isProxy = value; }
		}
		
		public string CallbackURL
		{
			get { return callbackUrl; }
			set { callbackUrl = value; }
		}
		
		string AdditionalUrlParams
		{
			get
			{
				return string.Format("&engine=unity&engine_v={0}&sdk=login&sdk_v={1}", Application.unityVersion, Constants.LoginSdkVersion);
			}
		}
		
		byte[] CryptoKey
		{
			get
			{
				return Encoding.ASCII.GetBytes(LoginID.Replace("-", string.Empty).Substring(0, 16));
			}
		}

		public string LastUserLogin
		{
			get
			{
				if (PlayerPrefs.HasKey(Constants.UserLogin) && !string.IsNullOrEmpty(loginId))
				{
					return PlayerPrefs.GetString(Constants.UserLogin);
				}
				
				return string.Empty;
			}
		}

		public string LastUserPassword
		{
			get
			{
				if (PlayerPrefs.HasKey(Constants.UserPassword) && !string.IsNullOrEmpty(loginId))
				{
					return Crypto.Decrypt(CryptoKey, PlayerPrefs.GetString(Constants.UserPassword));
				}
					
				return string.Empty;
			}
		}

		public string Token
		{
			get
			{
				return PlayerPrefs.HasKey(Constants.XsollaLoginToken) ? PlayerPrefs.GetString(Constants.XsollaLoginToken) : string.Empty;
			}
		}

		public string TokenExp
		{
			get
			{
				return PlayerPrefs.HasKey(Constants.XsollaLoginTokenExp) ? PlayerPrefs.GetString(Constants.XsollaLoginTokenExp) : string.Empty;
			}
		}

		public bool IsTokenValid
		{
			get
			{
				long epochTicks = new DateTime(1970, 1, 1).Ticks;
				long unixTime = ((DateTime.UtcNow.Ticks - epochTicks) / TimeSpan.TicksPerSecond);

				if (!string.IsNullOrEmpty(TokenExp))
				{
					return long.Parse(TokenExp) >= unixTime;
				}
				
				return false;
			}
		}
		
		public void SignOut()
		{
			if (PlayerPrefs.HasKey(Constants.XsollaLoginToken))
				PlayerPrefs.DeleteKey(Constants.XsollaLoginToken);
			if (PlayerPrefs.HasKey(Constants.XsollaLoginTokenExp))
				PlayerPrefs.DeleteKey(Constants.XsollaLoginTokenExp);
		}

		void SaveLoginPassword(string username, string password)
		{
			if (!string.IsNullOrEmpty(loginId))
			{
				PlayerPrefs.SetString(Constants.UserLogin, username);
				PlayerPrefs.SetString(Constants.UserPassword, Crypto.Encrypt(CryptoKey, password));
			}
		}
		public void Registration(string username, string password, string email, Action onSuccess, Action<ErrorDescription> onError)
		{
			WWWForm registrationForm = new WWWForm();
			registrationForm.AddField("username", username);
			registrationForm.AddField("password", password);
			registrationForm.AddField("email", email);

			string proxy = isProxy ? "proxy/registration" : "user";

			var urlBuilder = new StringBuilder(string.Format("https://login.xsolla.com/api/{0}?projectId={1}&login_url={2}", proxy, loginId, callbackUrl)).Append(AdditionalUrlParams);
			
			StartCoroutine(WebRequests.PostRequest(urlBuilder.ToString(), registrationForm, onSuccess, onError, ErrorDescription.RegistrationErrors));
		}
		public void ResetPassword(string username, Action onSuccess, Action<ErrorDescription> onError)
		{
			WWWForm form = new WWWForm();
			form.AddField("username", username);

			string proxy = isProxy ? "proxy/registration/password/reset" : "password/reset/request";

			var urlBuilder = new StringBuilder(string.Format("https://login.xsolla.com/api/{0}?projectId={1}", proxy, loginId)).Append(AdditionalUrlParams);
			
			StartCoroutine(WebRequests.PostRequest(urlBuilder.ToString(), form, onSuccess, onError, ErrorDescription.ResetPasswordErrors));
		}
		
		public void SignIn(string username, string password, bool rememberUser, Action<User> onSuccess, Action<ErrorDescription> onError)
		{
			WWWForm form = new WWWForm();
			form.AddField("username", username);
			form.AddField("password", password);
			form.AddField("remember_me", rememberUser.ToString());

			string proxy = isProxy ? "proxy/" : string.Empty;

			var urlBuilder = new StringBuilder(string.Format("https://login.xsolla.com/api/{0}login?projectId={1}&login_url={2}", proxy, loginId, callbackUrl)).Append(AdditionalUrlParams);

			StartCoroutine(WebRequests.PostRequest(urlBuilder.ToString(), form,
				(message) =>
				{
					if (isJwtValidationToken)
					{
						JWTValidation(message, onSuccess, onError);
					}
					else
					{
						if (onSuccess != null)
							onSuccess.Invoke(new User());
						if (rememberUser)
							SaveLoginPassword(username, password);
					}
				}, onError, ErrorDescription.LoginErrors));
		}

		void JWTValidation(string message, Action<User> onSuccess, Action<ErrorDescription> onError)
		{
			string token = ParseToken(message);
			
			if (!string.IsNullOrEmpty(token))
			{
				print(token);
				ValidateToken(token, (recievedMessage) =>
				{
					var user = new User();
					
					user = JsonUtility.FromJson<TokenJson>(recievedMessage).token_payload;
					PlayerPrefs.SetString(Constants.XsollaLoginTokenExp, user.exp);

					if (onSuccess != null)
						onSuccess.Invoke(user);
				}, onError);
			}
			else if (onError != null)
				onError.Invoke(new ErrorDescription(string.Empty, "Failed to parse token", Error.InvalidToken));
		}
		
		void ValidateToken(string token, Action<string> onRecievedToken, Action<ErrorDescription> onError)
		{
			WWWForm form = new WWWForm();
			form.AddField("token", token);
			StartCoroutine(WebRequests.PostRequest(jwtValidationUrl, form, onRecievedToken, onError, ErrorDescription.TokenErrors));
		}

		string ParseToken(string message)
		{
			Regex regex = new Regex(@"token=\S*[&#]");
			try
			{
				var match = regex.Match(message).Value.Replace("token=", string.Empty);
				match = match.Remove(match.Length - 1);
				var token = match;
				PlayerPrefs.SetString(Constants.XsollaLoginToken, token);
				return token;
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}
	}
}