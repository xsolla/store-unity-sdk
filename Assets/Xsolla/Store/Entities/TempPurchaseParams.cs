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

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Settings settings;

		[JsonProperty("custom_parameters", NullValueHandling = NullValueHandling.Ignore)]
		public Dictionary<string, object> custom_parameters;
		
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string currency;
		
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string locale;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? quantity;
		
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ShippingData shipping_data;
		
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Dictionary<string, object> shipping_method;

		[Serializable]
		public class Settings
		{
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string return_url;

			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public PayStationUI ui;
			
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public RedirectPolicy redirect_policy;
		}
	}
}
