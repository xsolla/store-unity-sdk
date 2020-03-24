using System;
using UnityEngine;
using System.Text;
using JetBrains.Annotations;
using Xsolla.Core;

namespace Xsolla.Login
{
	[PublicAPI]
	public partial class XsollaLogin : MonoSingleton<XsollaLogin>
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

		public string ShadowAccountUserID {
			get {
				return PlayerPrefs.HasKey(Constants.UserShadowAccount) ? PlayerPrefs.GetString(Constants.UserShadowAccount) : string.Empty;
			}
			set {
				PlayerPrefs.SetString(Constants.UserShadowAccount, value);
			}
		}

		public string ShadowAccountPlatform {
			get {
				return PlayerPrefs.HasKey(Constants.UserShadowPlatform) ? PlayerPrefs.GetString(Constants.UserShadowPlatform) : string.Empty;
			}
			set {
				PlayerPrefs.SetString(Constants.UserShadowPlatform, value);
			}
		}

		void SaveLoginPassword(string username, string password)
		{
			if (!string.IsNullOrEmpty(XsollaSettings.LoginId))
			{
				PlayerPrefs.SetString(Constants.UserLogin, username);
				PlayerPrefs.SetString(Constants.UserPassword, Crypto.Encrypt(CryptoKey, password));
			}
		}

		public void ValidateToken(string token, Action<User> onSuccess, Action<Core.Error> onError)
		{
			if (!string.IsNullOrEmpty(token))
			{
				WebRequestHelper.Instance.PostRequest<TokenJson, ValidateToken>(
					XsollaSettings.JwtValidationUrl, new ValidateToken(token),
					(response) => {
						TokenExp = response.token_payload.exp;
						onSuccess?.Invoke(response.token_payload);
					}, onError, Core.Error.TokenErrors);
			}
			else
				onError?.Invoke(new Core.Error(ErrorType.InvalidToken, string.Empty, "Failed to parse token"));
		}
	}
}