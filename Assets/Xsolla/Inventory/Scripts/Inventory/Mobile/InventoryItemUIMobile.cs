using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	[RequireComponent(typeof(InventoryItemUI))]
	public class InventoryItemUIMobile : MonoBehaviour
	{
		[SerializeField] private SimpleButton FullscreenButton = default;
		
		private ItemModel _itemInformation;

		private void Awake()
		{
			var itemUI = GetComponent<InventoryItemUI>();
			itemUI.OnInitialized += OnItemInitialized;
		}

		private void OnItemInitialized(ItemModel itemInformation)
		{
			_itemInformation = itemInformation;
			
			FullscreenButton.onClick += () =>
			{
				InventoryItemInfoMobile.ShowItem(itemInformation);
			};
		}
	}
}