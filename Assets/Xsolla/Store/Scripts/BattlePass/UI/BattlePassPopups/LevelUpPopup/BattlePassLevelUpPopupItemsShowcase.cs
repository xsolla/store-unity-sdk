using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class BattlePassLevelUpPopupItemsShowcase : MonoBehaviour
	{
		[SerializeField] private ItemUI FrontItem = default;
		[SerializeField] private GameObject ItemPrefab = default;
		[SerializeField] private Transform ItemsRoot = default;
		[Space]
		[SerializeField] private GameObject[] FoldedView = default;
		[SerializeField] private GameObject[] UnfoldedView = default;
		[Space]
		[SerializeField] private SimpleButton ViewAllButton = default;
		[SerializeField] private Text ViewAllText = default;
		[SerializeField] private GameObject ViewAllButtonHolder = default;
		[Space]
		[SerializeField] private SimpleButton CollapseListButton = default;

		private const string VIEW_ALL_TEMPLATE = "VIEW ALL (+{0} ITEMS)";

		private void Awake()
		{
			ViewAllButton.onClick += () => SwapActive(toActivate: UnfoldedView, toInactivate: FoldedView);
			CollapseListButton.onClick += () => SwapActive(toActivate: FoldedView, toInactivate: UnfoldedView);
		}

		public void ShowItems(BattlePassItemDescription[] items)
		{
			if (items == null || items.Length == 0)
			{
				Debug.LogError($"Items were null or empty");
				return;
			}

			SwapActive(toActivate: FoldedView, toInactivate: UnfoldedView);
			ClearPreviousItems();
			SetFrontItem(items[0]);
			SetViewAllButton(items.Length);

			if (items.Length > 1)
				SetItemsList(items);
		}

		private void ClearPreviousItems()
		{
			var childCount = ItemsRoot.childCount;

			if (childCount > 0)
			{
				var objectsToDelete = new List<GameObject>(childCount);

				for (int i = 0; i < childCount; i++)
					objectsToDelete.Add(ItemsRoot.GetChild(i).gameObject);

				foreach (var gameObject in objectsToDelete)
					Destroy(gameObject);
			}
		}

		private void SetFrontItem(BattlePassItemDescription item)
		{
			FrontItem.Initialize(item.ItemCatalogModel, item.Quantity);
		}

		private void SetViewAllButton(int itemsCount)
		{
			if (itemsCount > 1)
			{
				ViewAllText.text = string.Format(VIEW_ALL_TEMPLATE, itemsCount - 1);
				ViewAllButtonHolder.SetActive(true);
			}
			else
				ViewAllButtonHolder.SetActive(false);
		}

		private void SetItemsList(BattlePassItemDescription[] items)
		{
			foreach (var item in items)
			{
				var gameObject = Instantiate(ItemPrefab, ItemsRoot);
				var itemScript = gameObject.GetComponent<ItemUI>();
				itemScript.Initialize(item.ItemCatalogModel, item.Quantity);
			}
		}

		private void SwapActive(GameObject[] toActivate, GameObject[] toInactivate)
		{
			foreach (var gameObject in toActivate)
				gameObject.SetActive(true);

			foreach (var gameObject in toInactivate)
				gameObject.SetActive(false);
		}
	}
}
