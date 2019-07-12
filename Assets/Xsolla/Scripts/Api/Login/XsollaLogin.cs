using System;
using UnityEngine;
using System.Text;
using JetBrains.Annotations;
using Xsolla.Core;

namespace Xsolla.Login
{
	[PublicAPI]
	public class XsollaLogin : MonoSingleton<XsollaLogin>
	{
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
				return Encoding.ASCII.GetBytes(XsollaSettings.LoginId.Replace("-", string.Empty).Substring(0, 16));
			}
		}

		public string LastUserLogin
		{
			get
			{
				if (PlayerPrefs.HasKey(Constants.UserLogin) && !string.IsNullOrEmpty(XsollaSettings.LoginId))
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
				if (PlayerPrefs.HasKey(Constants.UserPassword) && !string.IsNullOrEmpty(XsollaSettings.LoginId))
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
			set
			{
				PlayerPrefs.SetString(Constants.XsollaLoginToken, value);
			}
		}

		public string TokenExp
		{
			get
			{
				return PlayerPrefs.HasKey(Constants.XsollaLoginTokenExp) ? PlayerPrefs.GetString(Constants.XsollaLoginTokenExp) : string.Empty;
			}
			set
			{
				PlayerPrefs.SetString(Constants.XsollaLoginTokenExp, value);
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
			if (!string.IsNullOrEmpty(XsollaSettings.LoginId))
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

			string proxy = XsollaSettings.UseProxy ? "proxy/registration" : "user";

			var urlBuilder = new StringBuilder(string.Format("https://login.xsolla.com/api/{0}?projectId={1}&login_url={2}", proxy, XsollaSettings.LoginId, XsollaSettings.CallbackUrl)).Append(AdditionalUrlParams);
			
			StartCoroutine(WebRequests.PostRequest(urlBuilder.ToString(), registrationForm, onSuccess, onError, ErrorDescription.RegistrationErrors));
		}
		public void ResetPassword(string username, Action onSuccess, Action<ErrorDescription> onError)
		{
			WWWForm form = new WWWForm();
			form.AddField("username", username);

			string proxy = XsollaSettings.UseProxy ? "proxy/registration/password/reset" : "password/reset/request";

			var urlBuilder = new StringBuilder(string.Format("https://login.xsolla.com/api/{0}?projectId={1}", proxy, XsollaSettings.LoginId)).Append(AdditionalUrlParams);
			
			StartCoroutine(WebRequests.PostRequest(urlBuilder.ToString(), form, onSuccess, onError, ErrorDescription.ResetPasswordErrors));
		}
		
		public void SignIn(string username, string password, bool rememberUser, Action<User> onSuccess, Action<ErrorDescription> onError)
		{
			WWWForm form = new WWWForm();
			form.AddField("username", username);
			form.AddField("password", password);
			form.AddField("remember_me", rememberUser.ToString());

			string proxy = XsollaSettings.UseProxy ? "proxy/" : string.Empty;

			var urlBuilder = new StringBuilder(string.Format("https://login.xsolla.com/api/{0}login?projectId={1}&login_url={2}", proxy, XsollaSettings.LoginId, XsollaSettings.CallbackUrl)).Append(AdditionalUrlParams);

			StartCoroutine(WebRequests.PostRequest(urlBuilder.ToString(), form,
				(response) =>
				{
					if (rememberUser)
					{
						SaveLoginPassword(username, password);
					}

					Token = ParseUtils.ParseToken(response);;
					
					if (XsollaSettings.UseJwtValidation)
					{
						ValidateToken(Token, onSuccess, onError);
					}
					else
					{
						if (onSuccess != null)
						{
							onSuccess.Invoke(new User());
						}
					}
				}, onError, ErrorDescription.LoginErrors));
		}

		void ValidateToken(string token, Action<User> onSuccess, Action<ErrorDescription> onError)
		{
			if (!string.IsNullOrEmpty(token))
			{
				WWWForm form = new WWWForm();
				form.AddField("token", token);
				
				StartCoroutine(WebRequests.PostRequest(XsollaSettings.JwtValidationUrl, form, (response =>
				{
					var user = JsonUtility.FromJson<TokenJson>(response).token_payload;
					
					TokenExp = user.exp;

					if (onSuccess != null)
					{
						onSuccess.Invoke(user);
					}
				}), onError, ErrorDescription.TokenErrors));
			}
			else if (onError != null)
			{
				onError.Invoke(new ErrorDescription(string.Empty, "Failed to parse token", Error.InvalidToken));
			}
		}
	}
}