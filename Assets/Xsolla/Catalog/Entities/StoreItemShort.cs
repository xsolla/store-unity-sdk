using System;
using Xsolla.Core;

namespace Xsolla.Catalog
{
	[Serializable]
	public class StoreItemShort
	{
		public string sku;
		public string name;
		public string description;
		public StoreItemGroup[] groups;
	}
}
