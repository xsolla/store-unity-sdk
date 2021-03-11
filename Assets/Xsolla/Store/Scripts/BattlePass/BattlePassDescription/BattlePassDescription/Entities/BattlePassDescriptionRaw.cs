using System;

namespace Xsolla.Demo
{
	[Serializable]
    public class BattlePassDescriptionRaw
    {
		public string Name;
		public string ExpiryDate;
		public BattlePassLevelDescriptionRaw[] Levels;
    }
}
