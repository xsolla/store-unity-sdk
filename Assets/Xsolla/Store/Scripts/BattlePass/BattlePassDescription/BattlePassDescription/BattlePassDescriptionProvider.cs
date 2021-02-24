using System;
using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassDescriptionProvider : MonoBehaviour
    {
		[SerializeField] private BaseBattlePassCatalogExtractor CatalogExtractor = default;
		[SerializeField] private BaseBattlePassJsonExtractor JsonExtractor = default;
		[SerializeField] private BaseBattlePassDescriptionConverter DescriptionConverter = default;
		[SerializeField] private BaseBattlePassCurrentDefiner CurrentDefiner = default;

		public event Action<BattlePassDescription> BattlePassDescriptionArrived;

		private void Awake()
		{
			CatalogExtractor.BattlePassItemsExtracted += JsonExtractor.OnBattlePassItemsExtracted;
			JsonExtractor.BattlePassJsonExtracted += DescriptionConverter.OnBattlePassJsonExtracted;
			DescriptionConverter.BattlePassDescriptionsConverted += CurrentDefiner.OnBattlePassDescriptionsConverted;
			CurrentDefiner.CurrentBattlePassDefined += this.OnCurrentBattlePassDefined;
		}

		public void GetBattlePassDescription()
		{
			CatalogExtractor.ExtractBattlePassItems();
		}

		private void OnCurrentBattlePassDefined(BattlePassDescription battlePassDescription)
		{
			BattlePassDescriptionArrived?.Invoke(battlePassDescription);
		}
	}
}
