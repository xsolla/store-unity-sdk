using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;
using JetBrains.Annotations;

namespace Xsolla.Login
{
	[PublicAPI]
	public partial class XsollaLogin : MonoSingleton<XsollaLogin>
	{
		public event Action TokenChanged;

		private string AnalyticUrlAddition => $"engine=unity&engine_v={Application.unityVersion}&sdk=login&sdk_v={Constants.LoginSdkVersion}";

		private List<WebRequestHeader> AnalyticHeaders
		{
			get
			{
				if (Application.platform != RuntimePlatform.WebGLPlayer)
				{
					return new List<WebRequestHeader>
					{
						new WebRequestHeader() { Name = "X-ENGINE", Value = "UNITY" },
						new WebRequestHeader() { Name = "X-ENGINE-V", Value = Application.unityVersion.ToUpper() },
						new WebRequestHeader() { Name = "X-SDK", Value = "LOGIN" },
						new WebRequestHeader() { Name = "X-SDK-V", Value = Constants.LoginSdkVersion.ToUpper() },
					};
				}
				else
					return new List<WebRequestHeader>();
			}
		}

		private List<WebRequestHeader> AppendAnalyticHeadersTo(params WebRequestHeader[] headers)
		{
			var analyticHeaders = AnalyticHeaders;
			var result = new List<WebRequestHeader>(capacity: analyticHeaders.Count + headers.Length);

			result.AddRange(headers);
			result.AddRange(analyticHeaders);

			return result;
		}

		private List<WebRequestHeader> AuthAndAnalyticHeaders => AppendAnalyticHeadersTo(WebRequestHeader.AuthHeader(Token));

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

		private Token _token;
		public Token Token
		{
			get => _token ?? (_token = new Token());
			set
			{
				_token = value;
				TokenChanged?.Invoke();
			}
		}
	}
}