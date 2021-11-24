using System;
using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassLevelUpDataProvider : MonoBehaviour
    {
		[SerializeField] private BaseBattlePassCatalogExtractor LevelUpExtractor;
		[SerializeField] private BattlePassLevelUpCurrentDefiner CurrentDefiner;

		private string _battlePassName;

		public event Action<CatalogItemModel> LevelUpUtilArrived;

		private void Awake()
		{
			LevelUpExtractor.BattlePassItemsExtracted += levelUpUtils => CurrentDefiner.DefineCurrent(levelUpUtils, _battlePassName);
			CurrentDefiner.CurrentLevelUpDefined += item =>
			{
				if (LevelUpUtilArrived != null)
					LevelUpUtilArrived.Invoke(item);
			};
		}

		public void OnBattlePassDescriptionArrived(string battlePassName)
		{
			_battlePassName = battlePassName;
			LevelUpExtractor.ExtractBattlePassItems();
		}
	}
}
