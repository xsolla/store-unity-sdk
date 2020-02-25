using System;
using System.Collections.Generic;
using Xsolla.PayStation;

namespace Xsolla.Store
{
	[Serializable]
	public class TempPurchaseParams
	{
		public class Settings
		{
			public class SettingsUI
			{
				public string theme;

				private static Dictionary<PaystationTheme, string> themes = new Dictionary<PaystationTheme, string>() {
					{ PaystationTheme.Dark, "dark" },
					{ PaystationTheme.Default, "default" },
					{ PaystationTheme.DefaultDark, "default_dark" }
				};

				public SettingsUI(PaystationTheme theme = PaystationTheme.Dark)
				{
					this.theme = themes[theme];
				}
			}

			public SettingsUI ui;

			public Settings(PaystationTheme theme = PaystationTheme.Dark)
			{
				ui = new SettingsUI(theme);
			}
		}

		public bool sandbox;
		public Settings settings;
	}
}