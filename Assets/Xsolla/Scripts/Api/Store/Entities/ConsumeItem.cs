using System;

namespace Xsolla.Store
{ 
	public class ConsumeItem
	{
		public string sku;
		public int? quantity;
		public string instance_id;

		public string ToJson()
		{
			return string.Format("{{ \"sku\":\"{0}\",\"quantity\":{1},\"instance_id\":{2}}}", 
				sku, quantity.HasValue ? quantity.Value.ToString() : "null", string.IsNullOrEmpty(instance_id) ? "null" : instance_id);
		}
	}
}