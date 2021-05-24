using System;
using Newtonsoft.Json;

namespace Xsolla.Store
{
	[Serializable]
	public class CreatePaymentTokenRequest
    {
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Settings settings;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public object custom_parameters;

		public Purchase purchase;


		public CreatePaymentTokenRequest(Purchase purchase, Settings settings = null, object custom_parameters = null)
		{
			this.settings = settings;
			this.custom_parameters = custom_parameters;
			this.purchase = purchase;
		}


		[Serializable]
		public class Settings : TempPurchaseParams.Settings
		{
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string currency;

			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string locale;

			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public bool? sandbox;

			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string external_id;

			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public int? payment_method;
		}


		[Serializable]
		public class Purchase
		{
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Item[] items;

			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public Description description;

			public Checkout checkout;

			public Purchase(Checkout checkout, Description description)
			{
				this.items = null;
				this.description = description;
				this.checkout = checkout;
			}

			public Purchase(Checkout checkout, Item[] items)
			{
				this.items = items;
				this.description = null;
				this.checkout = checkout;
			}

			[Serializable]
			public class Item
			{
				public string name;

				[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
				public string image_url;

				[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
				public string description;

				public Price price;

				[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
				public int? quantity;

				[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
				public bool? is_bonus;

				public Item(string name, Price price, string image_url = null, string description = null, int? quantity = null, bool? is_bonus = null)
				{
					this.name = name;
					this.image_url = image_url;
					this.description = description;
					this.price = price;
					this.quantity = quantity;
					this.is_bonus = is_bonus;
				}

				[Serializable]
				public class Price
				{
					public string amount;

					[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
					public string amount_before_discount;

					public Price(string amount, string amount_before_discount = null)
					{
						this.amount = amount;
						this.amount_before_discount = amount_before_discount;
					}
				}
			}

			[Serializable]
			public class Description
			{
				public string value;

				public Description(string value)
				{
					this.value = value;
				}
			}

			[Serializable]
			public class Checkout
			{
				public float amount;
				public string currency;

				public Checkout(float amount, string currency)
				{
					this.amount = amount;
					this.currency = currency;
				}
			}
		}
	}
}
