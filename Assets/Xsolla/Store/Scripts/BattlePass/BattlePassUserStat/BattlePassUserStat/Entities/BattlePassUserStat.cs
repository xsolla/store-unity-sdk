namespace Xsolla.Demo
{
	public class BattlePassUserStat
    {
		public readonly int Level;
		public readonly int Exp;
		public readonly int[] ObtainedFreeItems;
		public readonly int[] ObtainedPremiumItems;

		public BattlePassUserStat(int level, int exp, int[] obtainedFreeItems, int[] obtainedPremiumItems)
		{
			Level = level;
			Exp = exp;
			ObtainedFreeItems = obtainedFreeItems;
			ObtainedPremiumItems = obtainedPremiumItems;
		}
	}
}
