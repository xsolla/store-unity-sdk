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

		void SaveLoginPassword(string username, string password)
		{
			if (!string.IsNullOrEmpty(XsollaSettings.LoginId))
			{
				PlayerPrefs.SetString(Constants.UserLogin, username);
				PlayerPrefs.SetString(Constants.UserPassword, Crypto.Encrypt(CryptoKey, password));
			}
		}

		public Token Token { get; set; }
	}
}