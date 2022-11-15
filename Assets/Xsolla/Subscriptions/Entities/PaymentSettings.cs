using System;
using Newtonsoft.Json;

namespace Xsolla.Subscriptions
{
	[Serializable]
	public class PaymentSettings
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public bool? sandbox;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public UI ui;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string currency;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string locale;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string external_id;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? payment_method;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string return_url;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public RedirectPolicy redirect_policy;

		[Serializable]
		public class UI
		{
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string size;
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string theme;
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string version;
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Desktop desktop;
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Mobile mobile;
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string license_url;
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string mode;
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public UserAccount user_account;

			[Serializable]
			public class Desktop
			{
				[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
				public Header header;

				[Serializable]
				public class Header
				{
					[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
					public bool? is_visible;
					[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
					public bool? visible_logo;
					[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
					public bool? visible_name;
					[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
					public string type;
					[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
					public bool? close_button;
				}
			}

			[Serializable]
			public class Mobile
			{
				[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
				public string mode;
				[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
				public Footer footer;
				[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
				public Header header;

				[Serializable]
				public class Footer
				{
					[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
					public bool? is_visible;
				}
				[Serializable]
				public class Header
				{
					[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
					public bool? close_button;
				}
			}

			[Serializable]
			public class UserAccount
			{
				[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
				public EnableAndOrder history;
				[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
				public EnableAndOrder payment_accounts;
				[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
				public EnableAndOrder info;
				[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
				public EnableAndOrder subscriptions;

				[Serializable]
				public class EnableAndOrder
				{
					public bool enable;
					public int order;
				}
			}
		}

		[Serializable]
		public class RedirectPolicy
		{
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string redirect_conditions;
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public int? delay;
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string status_for_manual_redirection;
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string redirect_button_caption;
		}
	}
}