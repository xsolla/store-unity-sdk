namespace Xsolla.Demo
{
	public class LocalizationService
	{
		public string GetPurchaseLimitReachedTitle()
			=> "Item no longer available";
		
		public string GetPurchaseLimitReachedMessage()
			=> "You can’t buy more of this item";

		public string GetNotEnoughVirtualCurrencyTitle()
			=> "Not enough currency";

		public string GetNotEnoughVirtualCurrencyMessage()
			=> "You don’t have enough currency to buy this item. Just a little more and you can get it";
	}
}