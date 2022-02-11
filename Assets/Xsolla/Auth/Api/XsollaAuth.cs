using UnityEngine;
using Xsolla.Core;
using JetBrains.Annotations;

namespace Xsolla.Login
{
	[PublicAPI]
	public partial class XsollaAuth : MonoSingleton<XsollaAuth>
	{
		private const string DEFAULT_OAUTH_STATE = "xsollatest";
		private const string DEFAULT_REDIRECT_URI = "https://login.xsolla.com/api/blank";

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
