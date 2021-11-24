using System;

namespace Xsolla.Demo
{
	public class BattlePassCatalogExtractor : BaseBattlePassCatalogExtractor
	{
		protected override Func<CatalogItemModel, bool> ItemPredicate
		{
			get
			{
				return (item => !item.Name.Contains(BattlePassConstants.BATTLEPASS_UTIL_CONTAINS));
			}
		}
		protected override string WarningNoItemsFoundIdentifier
		{
			get
			{
				return "descriptions";
			}
		}
	}
}
