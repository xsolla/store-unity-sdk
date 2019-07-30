using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Store;

public class ItemUI : MonoBehaviour
{
	[SerializeField]
	Image itemImage;
	[SerializeField]
	GameObject loadingCircle;
	[SerializeField]
	Text itemName;
	[SerializeField]
	Text itemDescription;
	[SerializeField]
	SimpleTextButton buyButton;
	[SerializeField]
	AddToCartButton addToCartButton;

	Coroutine _loadingRoutine;
	StoreItem _itemInformation;
	StoreController _storeController;
	GroupsController _groupsController;

	Sprite _itemImage;
	
	Coroutine _checkStatusCor;

	void Awake()
	{
		_storeController = FindObjectOfType<StoreController>();
		_groupsController = FindObjectOfType<GroupsController>();

		var cartGroup = FindObjectOfType<CartGroupUI>();

		buyButton.onClick = (() =>
		{
			var purchaseParams = new PurchaseParams();
			purchaseParams.currency = _itemInformation.prices[0].currency;
			XsollaStore.Instance.BuyItem(XsollaSettings.StoreProjectId, _itemInformation.sku, data =>
			{
				XsollaStore.Instance.OpenPurchaseUi(data);
				_checkStatusCor = StartCoroutine(CheckOrderStatus(data.order_id));
			}, print);
		});

		addToCartButton.onClick = (bSelected =>
		{
			if (bSelected)
			{
				_storeController.CartModel.AddCartItem(_itemInformation);
				cartGroup.IncreaseCounter();
			}
			else
			{
				_storeController.CartModel.RemoveCartItem(_itemInformation.sku); 
				cartGroup.DecreaseCounter();
			}
		});
	}

	void OnDestroy()
	{
		if (_checkStatusCor != null)
		{
			StopCoroutine(_checkStatusCor);
		}
	}

	public void Initialize(StoreItem itemInformation)
	{
		_itemInformation = itemInformation;

		if (_itemInformation.prices.Length != 0)
		{
			buyButton.Text = string.Format("BUY FOR ${0}", _itemInformation.prices[0].amount);
		}

		itemName.text = _itemInformation.name;
		itemDescription.text = _itemInformation.description;
	}

	void OnEnable()
	{
		if (_itemImage == null && !string.IsNullOrEmpty(_itemInformation.image_url))
		{
			if (StoreController.ItemIcons.ContainsKey(_itemInformation.image_url))
			{
				loadingCircle.SetActive(false);
				itemImage.sprite = StoreController.ItemIcons[_itemInformation.image_url];
			}
			else
			{
				_loadingRoutine = StartCoroutine(LoadImage(_itemInformation.image_url));
			}
		}
	}

	public void Refresh()
	{
		addToCartButton.Select(_storeController.CartModel.CartItems.ContainsKey(_itemInformation.sku));
	}

	IEnumerator LoadImage(string url)
	{
		using (var www = new WWW(url))
		{
			yield return www;
			
			yield return new WaitForSeconds(Random.Range(0.0f, 1.5f));
			
			var sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));

			_itemImage = sprite;
			
			loadingCircle.SetActive(false);
			itemImage.sprite = sprite;

			if (!StoreController.ItemIcons.ContainsKey(url))
			{
				StoreController.ItemIcons.Add(url, sprite);
			}
		}
	}

	IEnumerator CheckOrderStatus(int orderId)
	{
		yield return new WaitForSeconds(5.0f);
		
		XsollaStore.Instance.CheckOrderStatus(XsollaSettings.StoreProjectId, orderId,status =>
		{
			if (status.Status != OrderStatusType.Paid)
			{
				print("Waiting for order to be processed...");
				_checkStatusCor = StartCoroutine(CheckOrderStatus(orderId));
			}
			else
			{
				print("Order was successfully processed!");
				XsollaStore.Instance.GetInventoryItems((items => { _storeController.inventory = items; }), print);
			}
		}, print);
	}
}