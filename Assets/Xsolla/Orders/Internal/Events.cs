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
		public Data data;
		public int status;
		public DateTime created_at;
	}

	[Serializable]
	public class Billing
	{
		public string notification_type;
		public PaymentDetails payment_details;
		public Purchase purchase;
		public Settings settings;
		public Transaction transaction;
	}

	[Serializable]
	public class CountryWht
	{
		public int amount;
		public string currency;
		public int percent;
	}

	[Serializable]
	public class Data
	{
		public string notification_type;
		public Settings settings;
		public User user;
		public Billing billing;
		public Item[] items;
		public Order order;
	}

	[Serializable]
	public class DirectWht
	{
		public int amount;
		public string currency;
		public int percent;
	}

	[Serializable]
	public class Item
	{
		public string amount;
		public bool is_pre_order;
		public int quantity;
		public string sku;
		public string type;
	}

	[Serializable]
	public class Order
	{
		public string amount;
		public object comment;
		public string currency;
		public string currency_type;
		public int id;
		public string invoice_id;
		public string mode;
		public string platform;
		public string status;
	}

	[Serializable]
	public class Payment
	{
		public int amount;
		public string currency;
	}

	[Serializable]
	public class PaymentDetails
	{
		public CountryWht country_wht;
		public DirectWht direct_wht;
		public Payment payment;
		public PaymentMethodFee payment_method_fee;
		public PaymentMethodSum payment_method_sum;
		public Payout payout;
		public string payout_currency_rate;
		public RepatriationCommission repatriation_commission;
		public SalesTax sales_tax;
		public UserAcquisitionFee user_acquisition_fee;
		public Vat vat;
		public XsollaBalanceSum xsolla_balance_sum;
		public XsollaFee xsolla_fee;
	}

	[Serializable]
	public class PaymentMethodFee
	{
		public double amount;
		public string currency;
	}

	[Serializable]
	public class PaymentMethodSum
	{
		public int amount;
		public string currency;
	}

	[Serializable]
	public class Payout
	{
		public double amount;
		public string currency;
	}

	[Serializable]
	public class Purchase
	{
		public Total total;
	}

	[Serializable]
	public class RepatriationCommission
	{
		public int amount;
		public string currency;
	}

	[Serializable]
	public class SalesTax
	{
		public int amount;
		public string currency;
		public int percent;
	}

	[Serializable]
	public class Settings
	{
		public int merchant_id;
		public int project_id;
	}

	[Serializable]
	public class Total
	{
		public int amount;
		public string currency;
	}

	[Serializable]
	public class Transaction
	{
		public int agreement;
		public int dry_run;
		public ulong id;
		public DateTime payment_date;
		public int payment_method;
		public string payment_method_name;
		public string payment_method_order_id;
	}

	[Serializable]
	public class User
	{
		public string email;
		public string id;
		public string ip;
		public string name;
		public string zip;
		public string country;
		public string external_id;
	}

	[Serializable]
	public class UserAcquisitionFee
	{
		public int amount;
		public string currency;
		public int percent;
	}

	[Serializable]
	public class Vat
	{
		public int amount;
		public string currency;
		public int percent;
	}

	[Serializable]
	public class XsollaBalanceSum
	{
		public int amount;
		public string currency;
	}

	[Serializable]
	public class XsollaFee
	{
		public double amount;
		public string currency;
	}
}