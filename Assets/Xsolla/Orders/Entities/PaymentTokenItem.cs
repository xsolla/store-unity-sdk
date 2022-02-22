namespace Xsolla.Orders
{
    public class PaymentTokenItem
    {
		public readonly string name;
		public readonly string amount;
		public readonly string amountBeforeDiscount;
		public readonly string imageUrl;
		public readonly string description;
		public readonly int? quantity;
		public readonly bool? isBouns;

		public PaymentTokenItem(string name, string amount,
			string amountBeforeDiscount = null, string imageUrl = null, string description = null, int? quantity = null, bool? isBouns = null)
		{
			this.name = name;
			this.amount = amount;
			this.amountBeforeDiscount = amountBeforeDiscount;
			this.imageUrl = imageUrl;
			this.description = description;
			this.quantity = quantity;
			this.isBouns = isBouns;
		}
	}
}
