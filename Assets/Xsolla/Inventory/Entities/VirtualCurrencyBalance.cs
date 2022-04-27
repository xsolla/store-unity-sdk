using System;

namespace Xsolla.Inventory
{
	[Serializable]
	public class VirtualCurrencyBalance
	{
		public string sku;
		public string type;
		public string name;
		public uint amount;
		public string description;
		public string image_url;
	}
}
