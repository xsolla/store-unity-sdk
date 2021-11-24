using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Demo;

public class DemoScript : MonoBehaviour
{
	[SerializeField] private InputField LoginInput = default(InputField);
	[SerializeField] private InputField PasswordInput = default(InputField);
	[SerializeField] private Button LoginButton = default(Button);
	[SerializeField] private Text TokenLabel = default(Text);
	[Space]
	[SerializeField] private GameObject ItemPrefab = default(GameObject);
	[SerializeField] private Transform InventoryRoot = default(Transform);
	[Space]
	[SerializeField] private InputField ItemIdInput = default(InputField);
	[SerializeField] private Button BuyButton = default(Button);

	private List<GameObject> _currentInventoryItems = new List<GameObject>();

	private void Awake()
	{
		LoginButton.onClick.AddListener(OnLoginButton);
		BuyButton.onClick.AddListener(OnBuyButton);
	}

	private void OnLoginButton()
	{
		var username = LoginInput.text;
		var password = PasswordInput.text;
		SdkLoginLogic.Instance.SignIn(username,password,false,OnSignIn,OnError);
	}

	private void OnSignIn(string token)
	{
		TokenLabel.text = token;
		Token.Instance = Token.Create(token);

		SdkInventoryLogic.Instance.GetInventoryItems(RefreshInventoryList,OnError);
	}

	private void RefreshInventoryList(List<InventoryItemModel> newInventoryItems)
	{
		foreach (var currentItem in _currentInventoryItems)
			Destroy(currentItem);

		foreach (var newItem in newInventoryItems)
		{
			var textValue = (newItem.RemainingUses.HasValue && newItem.RemainingUses > 1)
				? string.Format(" {0} x{1}", newItem.Name, newItem.RemainingUses)
				: string.Format(" {0}", newItem.Name);

			var itemObj = Instantiate(ItemPrefab, InventoryRoot);
			_currentInventoryItems.Add(itemObj);

			var itemText = itemObj.GetComponent<Text>();
			itemText.text = textValue;
		}
	}

	private void OnBuyButton()
	{
		var itemID = ItemIdInput.text;
		SdkCatalogLogic.Instance.GetCatalogVirtualItems(
			onSuccess: catalogItems => OnGetCatalog(catalogItems,itemID),
			onError: OnError);
	}

	private void OnGetCatalog(List<CatalogVirtualItemModel> catalogItems, string targetItemID)
	{
		var itemToBuy = default(CatalogVirtualItemModel);

		foreach (var catalogItem in catalogItems)
		{
			if (catalogItem.Sku.Equals(targetItemID))
			{
				itemToBuy = catalogItem;
				break;
			}
		}

		if (itemToBuy != null)
			SdkPurchaseLogic.Instance.PurchaseForRealMoney(itemToBuy,null,OnItemPurchase,OnError);
		else
			UnityEngine.Debug.LogError(string.Format("Could not find item with id '{0}'", targetItemID));
	}

	private void OnItemPurchase(CatalogItemModel purchasedItem)
	{
		UnityEngine.Debug.Log(string.Format("Successfully purchased item '{0}'",purchasedItem.Name));
		SdkInventoryLogic.Instance.GetInventoryItems(RefreshInventoryList,OnError);
	}

	private void OnError(Error error)
	{
		UnityEngine.Debug.LogError(error.ErrorType.ToString());
		UnityEngine.Debug.LogError(error.errorMessage);
	}
}
