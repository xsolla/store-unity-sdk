using System;
using System.Collections.Generic;

namespace Xsolla.Demo
{
	public class UserCartModel
	{
		private readonly List<UserCartItem> _items;

		public UserCartModel()
		{
			_items = new List<UserCartItem>();
		}

		private Predicate<UserCartItem> SearchPredicate(string sku) => cartItem => cartItem.Sku.Equals(sku);

		private UserCartItem FindCartItemBy(string sku) => GetCartItems().Find(SearchPredicate(sku));

		public UserCartItem GetItem(string sku)
		{
			return FindCartItemBy(sku);
		}

		private bool Exist(string sku)
		{
			return _items.Exists(SearchPredicate(sku));
		}

		public UserCartItem AddItem(string sku)
		{
			if (!Exist(sku))
			{
				_items.Add(new UserCartItem()
				{
					Sku = sku
				});
			}
			return GetItem(sku);
		}

		public bool RemoveItem(string sku)
		{
			if(!Exist(sku)) return false;
			UserCartItem cartItem = GetItem(sku);
			_items.Remove(cartItem);
			return true;
		}

		public UserCartItem IncreaseCountOf(string sku)
		{
			if (!Exist(sku)) return null;
			UserCartItem cartItem = GetItem(sku);
			cartItem.Quantity++;
			return cartItem;
		}

		public UserCartItem DecreaseCountOf(string sku)
		{
			if (!Exist(sku)) return null;

			UserCartItem cartItem = GetItem(sku);
			cartItem.Quantity--;
			if (cartItem.Quantity == 0)
			{
				RemoveItem(sku);
			}
			return cartItem;
		}

		public List<UserCartItem> GetCartItems()
		{
			return _items;
		}

		public void Clear()
		{
			_items.Clear();
		}

		public bool IsEmpty()
		{
			return GetCartItems().Count == 0;
		}
	}
}
