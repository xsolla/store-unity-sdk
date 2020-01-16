using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Xsolla.Core;
using Xsolla.PayStation;
using Xsolla.Store;

public class PayStationController : MonoBehaviour
{
	[SerializeField] SimpleTextButton buyCrystalsButton;
	[SerializeField] GameObject payStationDemoOverlay;
	[SerializeField] VirtualCurrencyContainer virtualCurrencyBalanceWidget;

	void Awake()
	{
		// PayStation demo setup
		Init();
	}

	void Init()
	{
		gameObject.AddComponent<ImageLoader>();
		
		GetToken(token =>
		{
			// TODO Store should use PayStation token instead of JWT to authenticate user
			XsollaStore.Instance.Token = token;
			
			UpdateVirtualCurrencies();
		});

		AddListeners();
	}

	void GetToken(Action<string> onComplete)
	{
		// TODO replace hardcoded JWT with PayStation token obtained from server
		onComplete.Invoke(
			"eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJleHAiOjE5NjIyMzQwNDgsImlzcyI6Imh0dHBzOi8vbG9naW4ueHNvbGxhLmNvbSIsImlhdCI6MTU2MjE0NzY0OCwidXNlcm5hbWUiOiJ4c29sbGEiLCJ4c29sbGFfbG9naW5fYWNjZXNzX2tleSI6IjA2SWF2ZHpDeEVHbm5aMTlpLUc5TmMxVWFfTWFZOXhTR3ZEVEY4OFE3RnMiLCJzdWIiOiJkMzQyZGFkMi05ZDU5LTExZTktYTM4NC00MjAxMGFhODAwM2YiLCJlbWFpbCI6InN1cHBvcnRAeHNvbGxhLmNvbSIsInR5cGUiOiJ4c29sbGFfbG9naW4iLCJ4c29sbGFfbG9naW5fcHJvamVjdF9pZCI6ImU2ZGZhYWM2LTc4YTgtMTFlOS05MjQ0LTQyMDEwYWE4MDAwNCIsInB1Ymxpc2hlcl9pZCI6MTU5MjR9.GCrW42OguZbLZTaoixCZgAeNLGH2xCeJHxl8u8Xn2aI");
	}

	void UpdateVirtualCurrencies()
	{
		XsollaStore.Instance.GetVirtualCurrencyList(
			XsollaSettings.StoreProjectId,
			currencies =>
			{
				// filter virtual currencies to display only crystals
				var filteredCurrencies = new VirtualCurrencyItems
				{
					items = new[] {currencies.items.ToList().SingleOrDefault(item => item.sku == "crystal")}
				};

				virtualCurrencyBalanceWidget.SetCurrencies(filteredCurrencies);

				UpdateVirtualCurrenciesBalance(() =>
				{
					payStationDemoOverlay.SetActive(false);
				});
			}, print);
	}

	void UpdateVirtualCurrenciesBalance(Action onComplete = null)
	{
		XsollaStore.Instance.GetVirtualCurrencyBalance(
			XsollaSettings.StoreProjectId,
			(balance) =>
			{
				virtualCurrencyBalanceWidget.SetCurrenciesBalance(balance);
				onComplete?.Invoke();
			},
			print);
	}

	void AddListeners()
	{
		buyCrystalsButton.onClick = () =>
		{
			Debug.Log("Buy Crystals button clicked!");
			
			// Launch purchase process
			XsollaStore.Instance.BuyItem(XsollaSettings.StoreProjectId, "crystal_pack_1", data =>
			{
				XsollaStore.Instance.OpenPurchaseUi(data);
				ProcessOrder(data.order_id, () => { UpdateVirtualCurrenciesBalance(); });
			}, print);
		};
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
			if ((status.Status != OrderStatusType.Paid) && (status.Status != OrderStatusType.Done))
			{
				print(string.Format("Waiting for order {0} to be processed...", orderId));
				StartCoroutine(CheckOrderStatus(orderId, onOrderPaid));
			}
			else
			{
				print(string.Format("Order {0} was successfully processed!", orderId));
				onOrderPaid?.Invoke();
			}
		}, print);
	}
}