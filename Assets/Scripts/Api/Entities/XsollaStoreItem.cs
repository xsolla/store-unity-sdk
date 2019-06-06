using System;

namespace Xsolla
{
	[Serializable]
	public class XsollaStoreItem
	{
		public string sku;
		public string[] groups;
		public string name;
		public string type;
		public bool is_free;
		public string long_description;
		public string description;
		public string image_url;
		public XsollaPrice[] prices;
	}
}