namespace Xsolla.Demo
{
	public class LocalizationService
	{
		public string GetPurchaseLimitReachedMessage()
			=> "You can’t buy more of this item";

		public string GetNotEnoughVirtualCurrencyMessage()
			=> "You don’t have enough currency to buy this item. Just a little more and you can get it";
	}
}