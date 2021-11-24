using System;

namespace Xsolla.Demo
{
	public class BattlePassLevelUpExtractor : BaseBattlePassCatalogExtractor
	{
		protected override Func<CatalogItemModel, bool> ItemPredicate
		{
			get
			{
				return (item => item.Name.Contains(BattlePassConstants.LEVELUP_UTIL_NAME_CONTAINS));
			}
		}
		protected override string WarningNoItemsFoundIdentifier
		{
			get
			{
				return "levelUp util";
			}
		}
	}
}
