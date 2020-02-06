using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.PayStation;
using Xsolla.Store;

public class PayStationController : MonoBehaviour
{
	// SKU of crystals virtual currency
	const string VirtualCurrencyCrystal = "crystal";
	// SKU of virtual currency package that contains 100 crystals
	const string CrystalPack = "crystal_pack_1";

	[SerializeField] SimpleTextButton buyCrystalsButton;
	[SerializeField] Text purchaseStatusText;
	
	[SerializeField] GameObject loadingScreen;
	[SerializeField] GameObject purchaseStatusWidget;
	
	[SerializeField] VirtualCurrencyContainer virtualCurrencyBalanceWidget;

	void Awake()
	{
		// PayStation demo setup
		Init();
	}

	void Init()
	{
		// Attach ImageLoader component to demo scene controller in order to be able to load images
		gameObject.AddComponent<ImageLoader>();
		
		// Obtain PayStation token to query store API
		GetToken(token =>
		{
			XsollaStore.Instance.Token = token;
			
			UpdateVirtualCurrencies();
		});

		// Demo UI setup
		AddListeners();
	}

	void GetToken(Action<string> onComplete)
	{
		XsollaPayStation.Instance.RequestToken(onComplete, null);
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
					items = new[] {currencies.items.ToList().SingleOrDefault(item => item.sku == VirtualCurrencyCrystal)}
				};

				virtualCurrencyBalanceWidget.SetCurrencies(filteredCurrencies);

				UpdateVirtualCurrenciesBalance(() =>
				{
					loadingScreen.SetActive(false);
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
			// Launch purchase process
			XsollaStore.Instance.BuyItem(XsollaSettings.StoreProjectId, CrystalPack, data =>
			{
				XsollaStore.Instance.OpenPurchaseUi(data);
				ProcessOrder(data.order_id, () =>
				{
					UpdateVirtualCurrenciesBalance(() =>
					{
						// Hide widget that displays current order status
						purchaseStatusWidget.SetActive(false);
					});
				});
			}, print);
		};
	}
	
	void ProcessOrder(int orderId, Action onOrderPaid = null)
	{
		// Begin order status polling
		StartCoroutine(CheckOrderStatus(orderId, onOrderPaid));
		
		// Activate widget that displays current order status
		UpdateOrderStatusDisplayText(OrderStatusType.New);
		purchaseStatusWidget.SetActive(true);
	}

	IEnumerator CheckOrderStatus(int orderId, Action onOrderPaid = null)
	{
		yield return new WaitForSeconds(3.0f);
		
		XsollaStore.Instance.CheckOrderStatus(XsollaSettings.StoreProjectId, orderId,status =>
		{
			UpdateOrderStatusDisplayText(status.Status);
			
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
	
	void UpdateOrderStatusDisplayText(OrderStatusType status)
	{
		purchaseStatusText.text = string.Format("PURCHASE STATUS: {0}", status.ToString().ToUpper());
	}
}