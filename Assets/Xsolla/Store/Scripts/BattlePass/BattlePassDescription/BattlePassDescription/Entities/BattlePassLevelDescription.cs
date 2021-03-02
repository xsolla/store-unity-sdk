namespace Xsolla.Demo
{
	public class BattlePassLevelDescription
    {
		public readonly int Tier;
		public readonly int Experience;
		public readonly BattlePassItemDescription FreeItem;
		public readonly BattlePassItemDescription PremiumItem;

		public BattlePassLevelDescription(int tier, int experience, BattlePassItemDescription freeItem, BattlePassItemDescription premiumItem)
		{
			Tier = tier;
			Experience = experience;
			FreeItem = freeItem;
			PremiumItem = premiumItem;
		}
	}
}
