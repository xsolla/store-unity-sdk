using System;

namespace Xsolla.Store
{
	[Serializable]
	public class StoreItemShort
	{
		public string sku;
		public string name;
		public string description;
		public StoreItem.Group[] groups;
	}
}