using System;

namespace Xsolla.Demo
{
	[Serializable]
	public class BattlePassLevelDescription
    {
		public int Tier;
		public int Experience;
		public BattlePassItemDescription FreeItem;
		public BattlePassItemDescription PremiumItem;
	}
}
