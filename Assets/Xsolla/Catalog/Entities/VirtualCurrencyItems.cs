using System;
using Xsolla.Core;

namespace Xsolla.Catalog
{
	[Serializable]
	public class VirtualCurrencyItems
	{
		public VirtualCurrencyItem[] items;
	}

	[Serializable]
	public class VirtualCurrencyItem : StoreItem { }
}