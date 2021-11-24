using System;
using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassDescriptionProvider : MonoBehaviour
    {
		[SerializeField] private BaseBattlePassCatalogExtractor CatalogExtractor;
		[SerializeField] private BaseBattlePassJsonExtractor JsonExtractor;
		[SerializeField] private BaseBattlePassDescriptionConverter DescriptionConverter;
		[SerializeField] private BaseBattlePassCurrentDefiner CurrentDefiner;

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
			if (BattlePassDescriptionArrived != null)
				BattlePassDescriptionArrived.Invoke(battlePassDescription);
		}
	}
}
