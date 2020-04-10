using System;
using System.Linq;
using UnityEngine.SocialPlatforms;
using Xsolla.Core;
using Xsolla.Store;

public class UserCartItem
{
	public StoreItem Item { get; set; }
	public int Quantity { get; set; }
	
	private float Price => Item.price.amount;

	public UserCartItem(StoreItem storeItem)
	{
		Item = storeItem;
		Quantity = 1;
	}
	public float TotalPrice => Price * Quantity;

	/// <summary>
	/// Software calculation of the discount for demo project;
	/// </summary>
	public float TotalDiscount
	{
		get
		{
			if (XsollaSettings.StoreProjectId != "44056")
			{
				return 0.0f;
			}
			
			if (IsInRange(Quantity, 2, 4))
			{
				return Price * Quantity * 0.1f;
			}
			
			if (IsInRange(Quantity, 5, 9))
			{
				return Price * Quantity * 0.25f;
			}
			
			if (IsInRange(Quantity, 10, Int32.MaxValue))
			{
				return Price * Quantity * 0.5f;
			}

			return 0.0f;
		}
	}
	
	static bool IsInRange(int value, int minimum, int maximum)
	{
		return value >= minimum && value <= maximum;
	}
}