using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core.Popup;

namespace Xsolla.Demo
{
	public class CartMenuController : MonoBehaviour
	{
		[SerializeField] private GameObject ItemPrefab;
		[SerializeField] private GameObject itemsHeader;
		[SerializeField] private ItemContainer itemsContainer;
		[SerializeField] private CartControls cartControls;
		[SerializeField] private GameObject emptyCartMessage;
		[SerializeField] private SimpleButton goToStoreButton;

		private readonly List<GameObject> _items = new List<GameObject>();

		protected virtual void Start()
		{
			if (ItemPrefab == null || itemsContainer == null || cartControls == null)
			{
				var message = "Cart prefab is broken. Some fields is null.";
				Debug.LogError(message);
				PopupFactory.Instance.CreateError()
					.SetMessage(message)
					.SetCallback(() => DemoController.Instance.SetState(MenuState.Main));
				return;
			}

			UserCart.Instance.ClearCartEvent += Refresh;
			UserCart.Instance.UpdateItemEvent += OnUpdateItemEvent;
			UserCart.Instance.RemoveItemEvent += OnRemoveItemEvent;

			goToStoreButton.onClick = () => DemoController.Instance.SetPreviousState();

			cartControls.OnClearCart = UserCart.Instance.Clear;
			cartControls.OnBuyCart = OnBuyCart;
			Refresh();
		}

		private void OnDestroy()
		{
			UserCart.Instance.ClearCartEvent -= Refresh;
			UserCart.Instance.UpdateItemEvent -= OnUpdateItemEvent;
			UserCart.Instance.RemoveItemEvent -= OnRemoveItemEvent;
		}

		private void OnUpdateItemEvent(UserCartItem item, int deltaCount)
		{
			Refresh();
		}
		private void OnRemoveItemEvent(UserCartItem item)
		{
			Refresh();
		}

		private void Refresh()
		{
			ClearCartItems();
			PutItemsToContainer(UserCart.Instance.GetItems());
			InitPrices();
		}

		private void ClearCartItems()
		{
			_items.ForEach(Destroy);
			_items.Clear();
		}

		private void InitPrices()
		{
			var totalPrice = UserCart.Instance.CalculateFullPrice();
			var discount = UserCart.Instance.CalculateCartDiscount();
			if (discount >= 0.01F)
			{
				totalPrice -= discount;
				cartControls.Initialize(totalPrice, discount);
			}
			else
				cartControls.Initialize(totalPrice);
		}

		private void OnBuyCart()
		{
			UserCart.Instance.Purchase(
				onSuccess: DemoController.Instance.SetPreviousState);
		}

		private void PutItemsToContainer(List<UserCartItem> items)
		{
			ShowEmptyMessage(!items.Any());
			ShowCartContent(items.Any());

			itemsContainer.Clear();
			items.ForEach(i =>
			{
				var go = itemsContainer.AddItem(ItemPrefab);
				go.GetComponent<CartItemUI>().Initialize(i);
				_items.Add(go);
			});
		}

		private void ShowEmptyMessage(bool showEmptyMessage)
		{
			emptyCartMessage.SetActive(showEmptyMessage);
		}

		private void ShowCartContent(bool showCartContent)
		{
			itemsHeader.SetActive(showCartContent);
			itemsContainer.gameObject.SetActive(showCartContent);
			cartControls.gameObject.SetActive(showCartContent);
		}
	}
}
