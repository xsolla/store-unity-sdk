using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassItemsManager : MonoBehaviour
	{
		[SerializeField] private GameObject LevelBlockPrefab;
		[SerializeField] private Transform ItemsRoot;
		[SerializeField] private int VisibleItems = 6;

		private List<BattlePassLevelBlock> _levelBlocks;
		private bool _isBattlePassExpired;
		private BattlePassLevelBlock _currentLevelBlock;
		private bool _isItemClickAllowed = false;
		private BattlePassUserStat _currentUserStat;
		private bool _currentUserPremiumStatus;

		private InitializationStep _initializationStep = InitializationStep.None;
		private InitializationStep Initialization
		{
			get
			{
				return _initializationStep;
			}
			set
			{
				if (value > _initializationStep)
					_initializationStep = value;
			}
		}

		public event Action<ItemSelectedEventArgs> ItemSelected;

		private void Awake()
		{
			BattlePassItem.ItemClick += OnItemClick;
		}

		private void OnDestroy()
		{
			BattlePassItem.ItemClick -= OnItemClick;
		}

		public void OnBattlePassDescriptionArrived(BattlePassDescription battlePassDescription)
		{
			_levelBlocks = new List<BattlePassLevelBlock>();

			foreach (var levelDescription in battlePassDescription.Levels)
			{
				var levelGameObject = Instantiate(LevelBlockPrefab, ItemsRoot);
				var levelBlockscript = levelGameObject.GetComponent<BattlePassLevelBlock>();
				levelBlockscript.Initialize(levelDescription);
				_levelBlocks.Add(levelBlockscript);
			}

			_isBattlePassExpired = battlePassDescription.IsExpired;
			Initialization = InitializationStep.Description;
		}

		public void OnUserStatArrived(BattlePassUserStat userStat)
		{
			StartCoroutine(SetItemsStateOnUserStat(userStat));
		}

		private IEnumerator SetItemsStateOnUserStat(BattlePassUserStat userStat)
		{
			yield return new WaitWhile(() => Initialization < InitializationStep.Description);

			_isItemClickAllowed = false;
			SetCurrentLevel(userStat.Level);
			SetItemsState(userStat.Level, userStat.ObtainedFreeItems, userStat.ObtainedPremiumItems);
			_currentUserStat = userStat;
			_isItemClickAllowed = true;

			if (Initialization == InitializationStep.Done)
				OnUserPremiumDefined(_currentUserPremiumStatus);
			else
				Initialization = InitializationStep.UserStat;
		}

		public void OnUserPremiumDefined(bool isPremiumUser)
		{
			StartCoroutine(SetItemsStateOnUserPremium(isPremiumUser));
		}

		private IEnumerator SetItemsStateOnUserPremium(bool isPremiumUser)
		{
			yield return new WaitWhile(() => Initialization < InitializationStep.UserStat);
			_isItemClickAllowed = false;
			_currentUserPremiumStatus = isPremiumUser;

			if (isPremiumUser)
			{
				foreach (var level in _levelBlocks)
				{
					if /*any*/(level.PremiumItem.ItemState == BattlePassItemState.PremiumLocked)
					{
						OnUserStatArrived(_currentUserStat);//This will overwrite PremiumLocked state for premium items
						break;
					}
				}
			}
			else//if (!isPremiumUser)
			{
				foreach (var level in _levelBlocks)
				{
					var premiumItem = level.PremiumItem;
					var premiumItemState = premiumItem.ItemState;

					switch (premiumItemState)
					{
						case BattlePassItemState.Collect:
						case BattlePassItemState.FutureLocked:
							premiumItem.SetState(BattlePassItemState.PremiumLocked);
							break;
						case BattlePassItemState.PremiumLocked:
						case BattlePassItemState.Collected:
						case BattlePassItemState.Empty:
							//Do nothing
							break;
						default:
							Debug.LogWarning(
								string.Format("Unexpected item state: '{0}' for premium item on level: '{1}'. Target state: PremiumLocked", premiumItemState, premiumItem.ItemDescription.Tier));
							break;
					}
				}
			}

			_isItemClickAllowed = true;

			Initialization = InitializationStep.Done;
			ForceItemClick();
		}

		public void ShowCurrentLevelLabel(bool show)
		{
			StartCoroutine(ShowCurrentLevelLabelCoroutine(show));
		}

		private IEnumerator ShowCurrentLevelLabelCoroutine(bool show)
		{
			yield return new WaitWhile(() => _levelBlocks == null || _currentLevelBlock == null);

			if (show == false)
			{
				//Prevent two or more level labels to be hidden
				foreach (var levelBlock in _levelBlocks)
					levelBlock.ShowLevelLabel(true);
			}

			//Last N items are always visible, so making their LevelLabel disappear breaks UI
			var count = _levelBlocks.Count;
			var index = _levelBlocks.IndexOf(_currentLevelBlock);

			if ((count - index) >= VisibleItems)
				_currentLevelBlock.ShowLevelLabel(show);
		}

		public BattlePassItemDescription[] GetFutureLockedItems(int levelDeltaLimit)
		{
			var result = new List<BattlePassItemDescription>();

			var count = _levelBlocks.Count;
			var currentIndex = _levelBlocks.IndexOf(_currentLevelBlock);
			var maxIndex = currentIndex + levelDeltaLimit;

			for (int i = currentIndex + 1; i < _levelBlocks.Count && i <= maxIndex; i++)
			{
				if (_levelBlocks[i].FreeItem.ItemState == BattlePassItemState.FutureLocked) //It may also be Empty
					result.Add(_levelBlocks[i].FreeItem.ItemDescription);

				if (_levelBlocks[i].PremiumItem.ItemState == BattlePassItemState.FutureLocked)
					result.Add(_levelBlocks[i].PremiumItem.ItemDescription);
			}

			return result.ToArray();
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
				BattlePassItemState itemState;

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

		private void ForceItemClick()
		{
			if (BattlePassItem.SelectedItem != null)
				BattlePassItem.SelectedItem.ForceItemClick();
			else
				_levelBlocks[_levelBlocks.Count-1].PremiumItem.ForceItemClick();
		}

		private void OnItemClick(BattlePassItemClickEventArgs itemInfo)
		{
			if (!_isItemClickAllowed)
			{
				Debug.LogError("Item OnClick is called prior to ItemsManager (re)initialization");
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

			if (ItemSelected != null)
				ItemSelected.Invoke(eventArgs);
		}

		private enum InitializationStep
		{
			None = 1,
			Description = 2,
			UserStat = 3,
			Done = 4
		}
	}
}
