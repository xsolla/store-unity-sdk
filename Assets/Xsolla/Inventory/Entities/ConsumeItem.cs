using System;

namespace Xsolla.Store
{
	[Serializable]
	public class ConsumeItem
	{
		public string sku;
		public int? quantity;
		public string instance_id;
	}
}