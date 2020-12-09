using System;
using System.Collections.Generic;

namespace Xsolla.Store
{
	[Serializable]
	public class VirtualCurrencyPackage : StoreItem
	{
		[Serializable]
		public class Content
		{
			public string sku;
			public string name;
			public string type;
			public string description;
			public string image_url;
			public int quantity;
		}

		public string bundle_type;
		public List<VirtualCurrencyPackage.Content> content;
	}
}