using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using Xsolla.Core;
using System.Runtime.InteropServices;

namespace Xsolla.Store
{
	[PublicAPI]
	public partial class XsollaStore : MonoSingleton<XsollaStore>
		public GameObject InAppBrowserPrefab;
		private GameObject InAppBrowserObject;

	{
		public string Token
		{
			set { PlayerPrefs.SetString(Constants.XsollaStoreToken, value); }
			get { return PlayerPrefs.GetString(Constants.XsollaStoreToken, string.Empty); }
		}

		private void OnDestroy()
		{
			if(InAppBrowserObject != null) {
				Destroy(InAppBrowserObject);
			}
		}

		string AdditionalUrlParams
		{
			get
			{
				return string.Format("?engine=unity&engine_v={0}&sdk=store&sdk_v={1}", Application.unityVersion, Constants.StoreSdkVersion);
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
		
		string GetCurrencyUrlParam(string currency)
		{
			if (string.IsNullOrEmpty(currency))
			{
				return string.Empty;
			}
			
			return string.Format("&currency={0}", currency);
		}
		[DllImport("__Internal")]
		private static extern void Purchase(string token, bool sandbox);
	}
}