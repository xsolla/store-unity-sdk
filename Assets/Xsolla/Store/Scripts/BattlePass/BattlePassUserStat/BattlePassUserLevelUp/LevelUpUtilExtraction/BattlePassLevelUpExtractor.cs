using System;

namespace Xsolla.Demo
{
	public class BattlePassLevelUpExtractor : BaseBattlePassCatalogExtractor
	{
		protected override Func<CatalogItemModel, bool> ItemPredicate => (item => item.Name.Contains(BattlePassConstants.LEVELUP_UTIL_NAME_CONTAINS));
		protected override string WarningNoItemsFoundIdentifier => "levelUp util";
	}
}
