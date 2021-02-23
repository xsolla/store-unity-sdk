using System;

namespace Xsolla.Demo
{
    public class BattlePassDescription
    {
		public readonly string Name;
		public readonly DateTime ExpiryDate;
		public readonly bool IsExpired;
		public readonly BattlePassLevelDescription[] Levels;

		public BattlePassDescription(string name, DateTime expiryDate, bool isExpired, BattlePassLevelDescription[] levels)
		{
			Name = name;
			ExpiryDate = expiryDate;
			IsExpired = isExpired;
			Levels = levels;
		}
    }
}
