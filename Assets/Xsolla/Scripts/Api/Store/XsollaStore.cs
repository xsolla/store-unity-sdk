using JetBrains.Annotations;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Store
{
	[PublicAPI]
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		public Token Token { get; set; }

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

		string GetPlatformUrlParam()
		{
			if(XsollaSettings.Platform == PlatformType.None) {
				return string.Empty;
			}
			return "&platform=" + XsollaSettings.Platform.GetString();
		}
	}
}