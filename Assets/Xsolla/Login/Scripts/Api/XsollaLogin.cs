using System.Text;
using UnityEngine;
using Xsolla.Core;
using JetBrains.Annotations;

namespace Xsolla.Login
{
	[PublicAPI]
	public partial class XsollaLogin : MonoSingleton<XsollaLogin>
	{
		private byte[] CryptoKey => Encoding.ASCII.GetBytes(XsollaSettings.LoginId.Replace("-", string.Empty).Substring(0, 16));

		private void Awake()
		{
			if (XsollaSettings.AuthorizationType == AuthorizationType.OAuth2_0)
				InitOAuth2_0();
		}

		public void DeleteToken(string key)
		{
			if (!string.IsNullOrEmpty(key) && PlayerPrefs.HasKey(key))
			{
				PlayerPrefs.DeleteKey(key);
			}
		}

		public void SaveToken(string key, string token)
		{
			if (!string.IsNullOrEmpty(token))
			{
				PlayerPrefs.SetString(key, token);
			}
		}

		string GetLocaleUrlParam(string locale)
		{
			if (string.IsNullOrEmpty(locale))
			{
				return string.Empty;
			}
			return string.Format("&locale={0}", locale);
		}
	}
}
