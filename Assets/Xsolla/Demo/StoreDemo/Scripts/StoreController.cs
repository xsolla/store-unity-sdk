using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Login;
using Xsolla.Store;


public class StoreController : MonoBehaviour
{
	[SerializeField]
	GameObject popupPrefab;
	
	GroupsController _groupsController;
	ItemsController _itemsController;
	ExtraController _extraController;
	ItemsTabControl _itemsTabControl;

	MessagePopup _popup;

	[HideInInspector]
	public InventoryItems inventory;
	public CartModel CartModel { get; private set; }

	public static Dictionary<string, Sprite> ItemIcons;

	const string DefaultStoreToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJleHAiOjE5NjIyMzQwNDgsImlzcyI6Imh0dHBzOi8vbG9naW4ueHNvbGxhLmNvbSIsImlhdCI6MTU2MjE0NzY0OCwidXNlcm5hbWUiOiJ4c29sbGEiLCJ4c29sbGFfbG9naW5fYWNjZXNzX2tleSI6IjA2SWF2ZHpDeEVHbm5aMTlpLUc5TmMxVWFfTWFZOXhTR3ZEVEY4OFE3RnMiLCJzdWIiOiJkMzQyZGFkMi05ZDU5LTExZTktYTM4NC00MjAxMGFhODAwM2YiLCJlbWFpbCI6InN1cHBvcnRAeHNvbGxhLmNvbSIsInR5cGUiOiJ4c29sbGFfbG9naW4iLCJ4c29sbGFfbG9naW5fcHJvamVjdF9pZCI6ImU2ZGZhYWM2LTc4YTgtMTFlOS05MjQ0LTQyMDEwYWE4MDAwNCIsInB1Ymxpc2hlcl9pZCI6MTU5MjR9.GCrW42OguZbLZTaoixCZgAeNLGH2xCeJHxl8u8Xn2aI";

	public Cart Cart { get; private set; }

	void Start()
	{
		_groupsController = FindObjectOfType<GroupsController>();
		_itemsController = FindObjectOfType<ItemsController>();
		_extraController = FindObjectOfType<ExtraController>();
		_itemsTabControl = FindObjectOfType<ItemsTabControl>();
		
		CartModel = new CartModel();
		
		ItemIcons = new Dictionary<string, Sprite>();

		if (FindObjectOfType<XsollaLogin>() != null)
		{
			print("Store demo starts. Use token obtained from Login: " + XsollaLogin.Instance.Token);
			XsollaStore.Instance.Token = XsollaLogin.Instance.Token;
		}
		else
		{
			print("Store demo starts. Use default hardcoded token: " + DefaultStoreToken);
			XsollaStore.Instance.Token = DefaultStoreToken;
		}

		XsollaStore.Instance.CreateNewCart(XsollaSettings.StoreProjectId, newCart => { Cart = newCart; }, ShowError);

		XsollaStore.Instance.GetListOfItems(XsollaSettings.StoreProjectId, InitStoreUi, ShowError);

		XsollaStore.Instance.GetInventoryItems(XsollaSettings.StoreProjectId,(items => { inventory = items; }), ShowError);
	}

	void InitStoreUi(StoreItems items)
	{
		_groupsController.CreateGroups(items);
		_itemsController.CreateItems(items);
		
		_itemsTabControl.Init();
		_extraController.Init();
		
		_groupsController.SelectDefault();
	}
	public void ProcessOrder(int orderId, Action onOrderPaid = null)
	{
		StartCoroutine(CheckOrderStatus(orderId, onOrderPaid));
	}

	IEnumerator CheckOrderStatus(int orderId, Action onOrderPaid = null)
	{
		yield return new WaitForSeconds(3.0f);
		
		XsollaStore.Instance.CheckOrderStatus(XsollaSettings.StoreProjectId, orderId,status =>
		{
			if (status.Status != OrderStatusType.Paid)
			{
				print(string.Format("Waiting for order {0} to be processed...", orderId));
				StartCoroutine(CheckOrderStatus(orderId, onOrderPaid));
			}
			else
			{
				print(string.Format("Order {0} was successfully processed!", orderId));

				ShowSuccess();
				
				XsollaStore.Instance.GetInventoryItems(XsollaSettings.StoreProjectId,(items =>
				{
					inventory = items;
					
					if (onOrderPaid != null)
					{
						onOrderPaid.Invoke();
					}
				}), ShowError);
			}
		}, ShowError);
	}
	
	void OnDestroy()
	{
		StopAllCoroutines();
	}

	public void ResetCart()
	{
		XsollaStore.Instance.CreateNewCart(XsollaSettings.StoreProjectId, newCart =>
		{
			Cart = newCart; 
			CartModel.Clear();

			var cartGroup = FindObjectOfType<CartGroupUI>();
			cartGroup.ResetCounter();

			_itemsController.RefreshActiveContainer();
		}, ShowError);
	}
	
	public void ShowSuccess()
	{
		if (_popup == null)
		{
			_popup = Instantiate(popupPrefab, gameObject.transform).GetComponent<MessagePopup>();
			_popup.ShowSuccess(() => { _popup = null; });
		}
		else
		{
			print("Error popup is already shown, so error only printed to console");
		}
	}

	public void ShowError(Xsolla.Core.Error error)
	{
		if (_popup == null)
		{
			_popup = Instantiate(popupPrefab, gameObject.transform).GetComponent<MessagePopup>();
			_popup.ShowError(error, () => { _popup = null; });
			
			print(error);
		}
		else
		{
			print("Error popup is already shown, so error only printed to console: " + error);
		}
	}
}