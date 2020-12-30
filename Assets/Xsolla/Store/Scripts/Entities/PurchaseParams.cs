using System.Collections.Generic;

namespace Xsolla.Store
{
	public class PurchaseParams
	{
		public string currency;
		public string locale;
		public Dictionary<string, object> customParameters;
	}
}