using System;
using System.Linq;

namespace Xsolla.Demo
{
	public class BattlePassCatalogExtractor : BaseBattlePassCatalogExtractor
	{
		protected override Func<CatalogItemModel, bool> ItemPredicate => (item => !item.Name.Contains(BattlePassConstants.BATTLEPASS_UTIL_CONTAINS));
		protected override string WarningNoItemsFoundIdentifier => "descriptions";
	}
}
