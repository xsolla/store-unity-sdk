using System.Collections.Generic;

namespace Xsolla.Store
{
	public class PurchaseParams
	{
		public string currency;
		public string locale;
		public int? quantity;
		public Dictionary<string, object> custom_parameters;
	}
}