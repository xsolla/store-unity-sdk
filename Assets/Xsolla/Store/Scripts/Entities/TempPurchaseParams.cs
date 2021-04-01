using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xsolla.Core;

namespace Xsolla.Store
{
	[Serializable]
	public class TempPurchaseParams
	{
		public bool sandbox;
		
		public Settings settings;

		[JsonProperty("custom_parameters", NullValueHandling = NullValueHandling.Ignore)]
		public Dictionary<string, object> customParameters;
		
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string currency;
		
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string locale;

		public class Settings
		{
			public SettingsUI ui;

			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string return_url;
			
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public RedirectPolicy redirect_policy;
			
			public class SettingsUI
			{
				public string theme;

				private static Dictionary<PaystationTheme, string> themes = new Dictionary<PaystationTheme, string>()
				{
					{PaystationTheme.Dark, "dark"},
					{PaystationTheme.Default, "default"},
					{PaystationTheme.DefaultDark, "default_dark"}
				};

				public SettingsUI(PaystationTheme theme = PaystationTheme.Dark)
				{
					this.theme = themes[theme];
				}
			}

			public Settings(PaystationTheme theme = PaystationTheme.Dark)
			{
				ui = new SettingsUI(theme);
			}
		}

		public TempPurchaseParams()
		{
			
		}
	}
}
