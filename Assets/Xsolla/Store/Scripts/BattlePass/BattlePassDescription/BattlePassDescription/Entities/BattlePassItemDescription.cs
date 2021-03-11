namespace Xsolla.Demo
{
	public class BattlePassItemDescription
    {
		public readonly string Sku;
		public readonly string Promocode;
		public readonly int Quantity;
		public readonly int Tier;
		public readonly bool IsPremium;
		public readonly bool IsFinal;

		public ItemModel ItemCatalogModel { get; set; }

		public BattlePassItemDescription(string sku, string promocode, int quantity, int tier, bool isPremium, bool isFinal)
		{
			Sku = sku;
			Promocode = promocode;
			Quantity = quantity;
			Tier = tier;
			IsPremium = isPremium;
			IsFinal = isFinal;
		}
    }
}
