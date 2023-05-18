using System.Collections.Generic;

namespace Xsolla.Core
{
	public class PurchaseParams
	{
		public string currency;
		public string locale;
		public int? quantity;
		public string external_id;
		public Dictionary<string, object> custom_parameters;
		public ShippingData shipping_data;
		public Dictionary<string, object> shipping_method;
	}
}