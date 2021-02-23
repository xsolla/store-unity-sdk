using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassItemsManager : MonoBehaviour
	{
		[SerializeField] private GameObject LevelBlockPrefab = default;
		[SerializeField] private Transform ItemsRoot = default;

		private List<BattlePassLevelBlock> _levelBlocks = new List<BattlePassLevelBlock>();
		private BattlePassLevelBlock _currentLevelBlock;

		public void OnBattlePassDescriptionArrived(BattlePassDescription battlePassDescription)
		{
			foreach (var levelDescription in battlePassDescription.Levels)
			{
				var levelGameObject = Object.Instantiate(LevelBlockPrefab, ItemsRoot);
				var levelBlockscript = levelGameObject.GetComponent<BattlePassLevelBlock>();
				levelBlockscript.Initialize(levelDescription);
				_levelBlocks.Add(levelBlockscript);
			}
		}

		public void OnUserStatArrived(BattlePassUserStat userStat)
		{
			SetCurrentLevel(userStat.Level);
			SetItemsState(userStat.Level, userStat.ObtainedFreeItems, userStat.ObtainedPremiumItems);
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
					itemState = BattlePassItemState.CollectAvailable;
				else
					itemState = BattlePassItemState.FutureLocked;

				_levelBlocks[i].SetItemState(isPremium: false, itemState);
				_levelBlocks[i].SetItemState(isPremium: true, itemState);
			}
		}

		private void SetItemsStateByObtained(int[] obtainedFreeItems, int[] obtainedPremiumItems)
		{
			foreach (var itemRecord in obtainedFreeItems)
				_levelBlocks[itemRecord - 1].SetItemState(isPremium: false, BattlePassItemState.Collected);

			foreach (var itemRecord in obtainedPremiumItems)
				_levelBlocks[itemRecord - 1].SetItemState(isPremium: true, BattlePassItemState.Collected);
		}
	}
}
