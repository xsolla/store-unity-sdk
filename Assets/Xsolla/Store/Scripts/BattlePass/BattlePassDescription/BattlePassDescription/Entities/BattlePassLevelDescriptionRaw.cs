using System;

namespace Xsolla.Demo
{
	[Serializable]
	public class BattlePassLevelDescriptionRaw
    {
		public int Tier;
		public int Experience;
		public BattlePassItemDescriptionRaw FreeItem;
		public BattlePassItemDescriptionRaw PremiumItem;
	}
}
