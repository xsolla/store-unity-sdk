using System;
using System.Collections.Generic;
using Xsolla.Core;

namespace Xsolla.Catalog
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
			public InventoryOptions inventory_options;
		}

		public string bundle_type;
		public List<VirtualCurrencyPackage.Content> content;
	}
}
