using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Store
{
	[PublicAPI]
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		public GameObject InAppBrowserPrefab;
		public Transform InAppBrowserParent;

		public string Token
		{
			set { PlayerPrefs.SetString(Constants.XsollaStoreToken, value); }
			get { return PlayerPrefs.GetString(Constants.XsollaStoreToken, string.Empty); }
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

		WWWForm RequestParams(PurchaseParams purchaseParams)
		{
			var form = new WWWForm();

			if (purchaseParams == null)
			{
				return form;
			}

			if (!string.IsNullOrEmpty(purchaseParams.currency))
			{
				form.AddField("currency", purchaseParams.currency);
			}
			if (!string.IsNullOrEmpty(purchaseParams.country))
			{
				form.AddField("country", purchaseParams.country);
			}
			if (!string.IsNullOrEmpty(purchaseParams.locale))
			{
				form.AddField("locale", purchaseParams.locale);
			}
			form.AddField("sandbox", XsollaSettings.IsSandbox.ToString().ToLower());

			return form;
		}
	}
}