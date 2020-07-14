using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

public class InventoryItemUI : MonoBehaviour
{
	private const int PLAYFAB_API_CONSUME_ITEMS_LIMIT = 25;

	[SerializeField] private Image itemImage;
	[SerializeField] private GameObject loadingCircle;
	[SerializeField] private Text itemName;
	[SerializeField] private Text itemDescription;
	[SerializeField] private GameObject itemQuantityImage;
	[SerializeField] private Text itemQuantityText;
	[SerializeField] private ConsumeButton consumeButton;

	private InventoryItemModel _itemInformation;
	private CatalogVirtualItemModel _catalogItem;
	private IDemoImplementation _demoImplementation;

	private void Awake()
	{
		DisableConsumeButton();
	}

	public void Initialize(InventoryItemModel itemInformation, IDemoImplementation demoImplementation)
	{
		_demoImplementation = demoImplementation;
		_itemInformation = itemInformation;
		_catalogItem = UserCatalog.Instance.VirtualItems
			.FirstOrDefault(c => c.Sku.Equals(itemInformation.Sku));

		itemName.text = _itemInformation.Name;
		if (_itemInformation.RemainingUses == null)
		{
			if (itemQuantityImage != null)
				itemQuantityImage.SetActive(false);
			else
			{
				if (itemQuantityText != null)
					itemQuantityText.text = string.Empty;
			}
		}
		else
			itemQuantityText.text = _itemInformation.RemainingUses.Value.ToString();

		if (_catalogItem == null) return;
		itemDescription.text = _catalogItem.Description;
		if (!string.IsNullOrEmpty(_catalogItem.ImageUrl))
			ImageLoader.Instance.GetImageAsync(_catalogItem.ImageUrl, LoadImageCallback);
		else
		{
			loadingCircle.SetActive(false);
			itemImage.sprite = null;
		}
	}

	private void LoadImageCallback(string url, Sprite image)
	{
		loadingCircle.SetActive(false);
		itemImage.sprite = image;

		RefreshConsumeButton();
	}

	private void RefreshConsumeButton()
	{
		if (_itemInformation.IsConsumable)
		{
			EnableConsumeButton();
		}
		else
		{
			DisableConsumeButton();
		}
	}

	private void EnableConsumeButton()
	{
		consumeButton.gameObject.SetActive(true);
		consumeButton.onClick = ConsumeHandler;
		if (consumeButton.counter < 1)
			consumeButton.counter.IncreaseValue(1 - consumeButton.counter.GetValue());
		consumeButton.counter.ValueChanged += Counter_ValueChanged;
	}

	private void DisableConsumeButton()
	{
		consumeButton.counter.ValueChanged -= Counter_ValueChanged;
		consumeButton.gameObject.SetActive(false);
	}

	private void Counter_ValueChanged(int newValue)
	{
		if (newValue > _itemInformation.RemainingUses || newValue > PLAYFAB_API_CONSUME_ITEMS_LIMIT)
		{
			StartCoroutine(DecreaseConsumeQuantityCoroutine());
		}
	}

	private IEnumerator DecreaseConsumeQuantityCoroutine()
	{
		yield return new WaitForEndOfFrame();
		consumeButton.counter.DecreaseValue(1);
	}

	private void ConsumeHandler()
	{
		loadingCircle.SetActive(true);
		DisableConsumeButton();
		_demoImplementation.ConsumeInventoryItem(_itemInformation, (uint) consumeButton.counter.GetValue(),
			_ => ConsumeItemsSuccess(), _ => ConsumeItemsFailed());
	}

	private void ConsumeItemsSuccess()
	{
		EnableConsumeButton();
		loadingCircle.SetActive(false);
		UserInventory.Instance.Refresh();
	}

	private void ConsumeItemsFailed()
	{
		EnableConsumeButton();
		loadingCircle.SetActive(false);
	}
}