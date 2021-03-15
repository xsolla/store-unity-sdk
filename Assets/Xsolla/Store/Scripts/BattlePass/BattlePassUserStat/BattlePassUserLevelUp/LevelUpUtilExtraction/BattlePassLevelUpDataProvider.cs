using System;
using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassLevelUpDataProvider : MonoBehaviour
    {
		[SerializeField] private BaseBattlePassCatalogExtractor LevelUpExtractor = default;
		[SerializeField] private BattlePassLevelUpCurrentDefiner CurrentDefiner = default;

		private string _battlePassName;

		public event Action<CatalogItemModel> LevelUpUtilArrived;

		private void Awake()
		{
			LevelUpExtractor.BattlePassItemsExtracted += levelUpUtils => CurrentDefiner.DefineCurrent(levelUpUtils, _battlePassName);
			CurrentDefiner.CurrentLevelUpDefined += item => LevelUpUtilArrived?.Invoke(item);
		}

		public void OnBattlePassDescriptionArrived(string battlePassName)
		{
			_battlePassName = battlePassName;
			LevelUpExtractor.ExtractBattlePassItems();
		}
	}
}
