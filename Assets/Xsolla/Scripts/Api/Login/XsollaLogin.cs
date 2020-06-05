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
		private string AdditionalUrlParams => $"&engine=unity&engine_v={Application.unityVersion}&sdk=login&sdk_v={Constants.LoginSdkVersion}";

		private byte[] CryptoKey => Encoding.ASCII.GetBytes(XsollaSettings.LoginId.Replace("-", string.Empty).Substring(0, 16));

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

		public bool LoadToken(string key, out string token)
		{
			token = PlayerPrefs.HasKey(key) ? PlayerPrefs.GetString(key) : string.Empty;
			if (string.IsNullOrEmpty(token))
			{
				return false;
			}
			Token t;
			try
			{
				t = new Token(token);
			}
			catch (Exception e)
			{
				Debug.LogError($"Can't create {key} token = {token}. Exception:" + e.Message);
				PlayerPrefs.DeleteKey(key);
				token = string.Empty;
				return false;
			}
			
			if (t.SecondsLeft() >= 300) return true;
			PlayerPrefs.DeleteKey(key);
			token = string.Empty;
			return false;
		}

		public Token Token { get; set; }
	}
}