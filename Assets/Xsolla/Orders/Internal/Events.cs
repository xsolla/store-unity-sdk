using System;

namespace Xsolla.Orders
{
	[Serializable]
	public class Events
	{
		public Event[] events;
	}

	[Serializable]
	public class Event
	{
		public int id;
		public int status;
		public DateTime created_at;
		public EventData data;
	}

	[Serializable]
	public class EventData
	{
		// Common data
		public string notification_type;
		public Settings settings;
		public User user;

		// Purchase data
		public Transaction transaction;
		public PaymentDetails payment_details;
		public Purchase purchase;
	}

	[Serializable]
	public class Settings
	{
		public int project_id;
		public int merchant_id;
	}

	[Serializable]
	public class User
	{
		public string id;
		public string ip;
		public string phone;
		public string email;
		public string name;
		public string country;
		public string zip;
	}

	[Serializable]
	public class PaymentDetails
	{
		public Money payment;
		public Money payment_method_sum;
		public Money xsolla_balance_sum;
		public Money payout;
		public Money vat;
		public string payout_currency_rate;
		public Money country_wht;
		public Money user_acquisition_fee;
		public Money xsolla_fee;
		public Money payment_method_fee;
		public Money sales_tax;
		public Money direct_wht;
		public Money repatriation_commission;
	}

	[Serializable]
	public class Purchase
	{
		public Money total;
		public Money checkout;
		public Subscription subscription;
		public Gift gift;
		public Promotion[] promotions;
		public Coupon coupon;
		public Order order;
	}

	[Serializable]
	public class Subscription
	{
		public string plan_id;
		public int subscription_id;
		public string product_id;
		public string[] tags;
		public string date_create;
		public string date_next_charge;
		public string currency;
		public float amount;
	}

	[Serializable]
	public class Gift
	{
		public string giver_id;
		public string receiver_id;
		public string receiver_email;
		public string message;
		public string hide_giver_from_receiver;
	}

	[Serializable]
	public class Promotion
	{
		public string technical_name;
		public int id;
	}

	[Serializable]
	public class Coupon
	{
		public string coupon_code;
		public string campaign_code;
	}

	[Serializable]
	public class Order
	{
		public int id;
		public LineItems lineitems;
	}

	[Serializable]
	public class LineItems
	{
		public string sku;
		public int quantity;
		public Money price;
	}

	[Serializable]
	public class Transaction
	{
		public int id;
		public string external_id;
		public string payment_date;
		public int payment_method;
		public string payment_method_name;
		public string payment_method_order_id;
		public int dry_run;
		public long agreement;
	}

	[Serializable]
	public class Money
	{
		public string currency;
		public float amount;
		public float percent;
	}
}