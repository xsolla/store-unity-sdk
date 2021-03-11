using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassItemsManager : MonoBehaviour
	{
		[SerializeField] private GameObject LevelBlockPrefab = default;
		[SerializeField] private Transform ItemsRoot = default;
		[SerializeField] private int VisibleItems = 6;

		private List<BattlePassLevelBlock> _levelBlocks = new List<BattlePassLevelBlock>();
		private bool _isBattlePassExpired;
		private BattlePassLevelBlock _currentLevelBlock;
		private bool _isInitialized = false;

		public event Action<ItemSelectedEventArgs> ItemSelected;

		public void OnBattlePassDescriptionArrived(BattlePassDescription battlePassDescription)
		{
			foreach (var levelDescription in battlePassDescription.Levels)
			{
				var levelGameObject = Instantiate(LevelBlockPrefab, ItemsRoot);
				var levelBlockscript = levelGameObject.GetComponent<BattlePassLevelBlock>();
				levelBlockscript.Initialize(levelDescription);
				_levelBlocks.Add(levelBlockscript);
			}

			_isBattlePassExpired = battlePassDescription.IsExpired;
		}

		public void OnUserStatArrived(BattlePassUserStat userStat)
		{
			_isInitialized = false;
			SetCurrentLevel(userStat.Level);
			SetItemsState(userStat.Level, userStat.ObtainedFreeItems, userStat.ObtainedPremiumItems);
			_isInitialized = true;
			ForceItemClickOnInitialize();
		}

		public void ShowCurrentLevelLabel(bool show)
		{
			//Last N items are always visible, so making their LevelLabel disappear breaks UI
			var count = _levelBlocks.Count;
			var index = _levelBlocks.IndexOf(_currentLevelBlock);

			if ((count - index) >= VisibleItems)
				_currentLevelBlock.ShowLevelLabel(show);
		}

		private void SetCurrentLevel(int userLevel)
		{
			if (_currentLevelBlock != null)
				_currentLevelBlock.SetCurrent(false);

			var levelIndex = userLevel - 1;
			var newCurrentLevel = _levelBlocks[levelIndex];
			newCurrentLevel.SetCurrent(true);
			_currentLevelBlock = newCurrentLevel;
		}

		private void SetItemsState(int userLevel, int[] obtainedFreeItems, int[] obtainedPremiumItems)
		{
			SetItemsStateByUserLevel(userLevel);
			SetItemsStateByObtained(obtainedFreeItems, obtainedPremiumItems);
		}

		private void SetItemsStateByUserLevel(int userLevel)
		{
			var currentLevelIndex = userLevel - 1;

			for (int i = 0; i < _levelBlocks.Count; i++)
			{
				BattlePassItemState itemState = default;

				if (i <= currentLevelIndex)
					itemState = BattlePassItemState.Collect;
				else
					itemState = BattlePassItemState.FutureLocked;

				_levelBlocks[i].FreeItem.SetState(itemState);
				_levelBlocks[i].PremiumItem.SetState(itemState);
			}
		}

		private void SetItemsStateByObtained(int[] obtainedFreeItems, int[] obtainedPremiumItems)
		{
			foreach (var itemRecord in obtainedFreeItems)
				_levelBlocks[itemRecord - 1].FreeItem.SetState(BattlePassItemState.Collected);

			foreach (var itemRecord in obtainedPremiumItems)
				_levelBlocks[itemRecord - 1].PremiumItem.SetState(BattlePassItemState.Collected);
		}

		private void ForceItemClickOnInitialize()
		{
			if (BattlePassItem.SelectedItem != null)
				BattlePassItem.SelectedItem.ForceItemClick();
			else
				_levelBlocks[_levelBlocks.Count-1].PremiumItem.ForceItemClick();
		}

		private void Awake()
		{
			BattlePassItem.ItemClick += OnItemClick;
		}

		private void OnDestroy()
		{
			BattlePassItem.ItemClick -= OnItemClick;
		}

		private void OnItemClick(BattlePassItemClickEventArgs itemInfo)
		{
			if (!_isInitialized)
			{
				Debug.LogError($"Item OnClick is called prior to ItemsManager (re)initialization");
				return;
			}

			var itemsReadyToCollect = new List<BattlePassItemDescription>();

			foreach (var levelBlock in _levelBlocks)
			{
				if (levelBlock.FreeItem.ItemState == BattlePassItemState.Collect)
					itemsReadyToCollect.Add(levelBlock.FreeItem.ItemDescription);

				if (levelBlock.PremiumItem.ItemState == BattlePassItemState.Collect)
					itemsReadyToCollect.Add(levelBlock.PremiumItem.ItemDescription);
			}

			var eventArgs = new ItemSelectedEventArgs
				(
					selectedItemInfo: itemInfo,
					allItemsInCollectState: itemsReadyToCollect.ToArray(),
					isBattlePassExpired: _isBattlePassExpired
				);

			ItemSelected?.Invoke(eventArgs);
		}
	}
}
