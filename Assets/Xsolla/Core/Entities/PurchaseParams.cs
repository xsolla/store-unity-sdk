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
		public int? payment_method;
		public bool? close_button;
		public string close_button_icon;
		public bool? google_pay_quick_payment_button;
	}
}