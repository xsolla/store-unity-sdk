using System;
using System.Collections.Generic;
using System.Linq;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class UserCart : MonoSingleton<UserCart>
	{
		public event Action<UserCartItem> AddItemEvent;
		public event Action<UserCartItem> RemoveItemEvent;
		public event Action<UserCartItem, int> UpdateItemEvent;
		public event Action PurchaseCartEvent;
		public event Action ClearCartEvent;

		public bool IsUpdated { get; private set; } = true;

		private UserCartModel _cart;

		private IStoreDemoImplementation _demoImplementation;

		private int _pendingCartRequestsCount = 0;

		public void Init(IStoreDemoImplementation demoImplementation)
		{
			IsUpdated = false;

			_demoImplementation = demoImplementation;

			_cart = new UserCartModel();
		}

		public void Refresh(Action onSuccess = null, Action<Error> onError = null)
		{
			IsUpdated = false;
			_demoImplementation.GetCartItems(cart =>
			{
				_cart = cart;
				IsUpdated = true;
				onSuccess?.Invoke();
			}, onError);
		}

		public bool Contains(string sku)
		{
			return _cart.GetCartItems().Any(i => i.Sku.Equals(sku));
		}

		public void AddItem(string sku)
		{
			IsUpdated = false;
			_pendingCartRequestsCount++;
			_demoImplementation.UpdateCartItem(sku, 1, () =>
			{
				_pendingCartRequestsCount--;
				if (_pendingCartRequestsCount == 0)
				{
					Refresh();
				}
			});
			
			UserCartItem cartItem = _cart.AddItem(sku);
			if (cartItem != null)
				AddItemEvent?.Invoke(cartItem);
		}

		public void RemoveItem(string sku)
		{
			IsUpdated = false;
			_pendingCartRequestsCount++;
			_demoImplementation.RemoveCartItem(sku, () =>
			{
				_pendingCartRequestsCount--;
				if (_pendingCartRequestsCount == 0)
				{
					Refresh();
				}
			});
			
			var cartItem = _cart.GetItem(sku);
			if (!_cart.RemoveItem(sku)) return;
			if (cartItem.Quantity > 1)
				UpdateItemEvent?.Invoke(cartItem, (-1) * (cartItem.Quantity - 1));
			RemoveItemEvent?.Invoke(cartItem);
		}

		public void IncreaseCountOf(string sku)
		{
			var cartItem = _cart.IncreaseCountOf(sku);
			if (cartItem != null)
			{
				UpdateItemEvent?.Invoke(cartItem, 1);
			}
		}

		public void DecreaseCountOf(string sku)
		{
			UserCartItem cartItem = _cart.DecreaseCountOf(sku);
			if (cartItem == null) return;
			if (cartItem.Quantity == 0)
			{
				RemoveItemEvent?.Invoke(cartItem);
			}
			else
			{
				UpdateItemEvent?.Invoke(cartItem, -1);
			}
		}

		public List<UserCartItem> GetItems()
		{
			return _cart.GetCartItems();
		}

		public void Clear()
		{
			var items = GetItems();
			_cart.Clear();
			items.ForEach(i => RemoveItemEvent?.Invoke(i));
			ClearCartEvent?.Invoke();
		}

		public bool IsEmpty()
		{
			return _cart.IsEmpty();
		}

		public float CalculateRealPrice()
		{
			if (IsEmpty())
			{
				return 0.0F;
			}

			return CalculateFullPrice() - CalculateCartDiscount();
		}

		public float CalculateFullPrice()
		{
			return GetItems().Sum(i => i.TotalPrice);
		}

		public float CalculateCartDiscount()
		{
			return 99.99f; //GetItems().Sum(i => i.TotalDiscount);
		}

		public void Purchase(Action onSuccess = null, Action<Error> onError = null)
		{
			DemoController.Instance.StoreDemo.PurchaseCart(GetItems(), _ =>
			{
				onSuccess?.Invoke();
				PurchaseCartEvent?.Invoke();
			}, onError);
		}
	}
}