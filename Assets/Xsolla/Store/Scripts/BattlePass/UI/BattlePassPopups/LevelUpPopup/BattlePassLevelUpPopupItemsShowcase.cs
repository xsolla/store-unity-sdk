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
		private ShowState _currentState = ShowState.Neither;
		private int _currentItemsCount = 0;

		private void Awake()
		{
			ViewAllButton.onClick += () => SetState(ShowState.Unfolded);
			CollapseListButton.onClick += () => SetState(ShowState.Folded);
		}

		public void ShowItems(BattlePassItemDescription[] items)
		{
			_currentItemsCount = items?.Length ?? 0;

			if (items == null || items.Length == 0)
				SetState(ShowState.Neither);
			else
			{
				SetFrontItem(items[0]);
				ClearPreviousItems();
				SetItemsList(items);

				if (_currentState == ShowState.Neither)
					SetState(ShowState.Folded);
				else
					SetViewAllButton(_currentState);
			}
		}

		private void SetState(ShowState showState)
		{
			switch (showState)
			{
				case ShowState.Folded:
					SwapActive(toActivate: FoldedView, toInactivate: UnfoldedView);
					break;
				case ShowState.Unfolded:
					SwapActive(toActivate: UnfoldedView, toInactivate: FoldedView);
					break;
				case ShowState.Neither:
					SetActive(FoldedView, false);
					SetActive(UnfoldedView, false);
					break;
			}

			_currentState = showState;
			SetViewAllButton(showState);
		}

		private void SetViewAllButton(ShowState showState)
		{
			ViewAllText.text = string.Format(VIEW_ALL_TEMPLATE, _currentItemsCount - 1);

			if (showState == ShowState.Folded && _currentItemsCount > 1)
				ViewAllButtonHolder.SetActive(true);
			else
				ViewAllButtonHolder.SetActive(false);
		}

		private void SetFrontItem(BattlePassItemDescription item)
		{
			FrontItem.Initialize(item.ItemCatalogModel, item.Quantity);
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
			SetActive(toActivate, true);
			SetActive(toInactivate, false);
		}

		private void SetActive(GameObject[] gameObjects, bool targetState)
		{
			foreach (var item in gameObjects)
			{
				if (item.activeSelf != targetState)
					item.SetActive(targetState);
			}
		}

		private enum ShowState
		{
			Folded,
			Unfolded,
			Neither
		}
	}
}
