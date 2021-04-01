using System;

namespace Xsolla.Demo
{
	[Serializable]
	public abstract class ItemModel
	{
		public string Sku;
		public string Name;
		public string Description;
		public string LongDescription;
		public string ImageUrl;
		public bool IsConsumable;

		public abstract bool IsVirtualCurrency();
		public abstract bool IsSubscription();
		public abstract bool IsBundle();
	}
}