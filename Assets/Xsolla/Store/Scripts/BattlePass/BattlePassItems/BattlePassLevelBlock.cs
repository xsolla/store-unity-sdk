using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class BattlePassLevelBlock : MonoBehaviour
    {
		[SerializeField] private Text LevelLabel = default;
		[SerializeField] private BattlePassItemUI FreeItem = default;
		[SerializeField] private BattlePassItemUI PremiumItem = default;

		public void Initialize(BattlePassLevelDescription levelDescription)
		{
			LevelLabel.text = levelDescription.Tier.ToString();
			FreeItem.Initialize(levelDescription.FreeItem);
			PremiumItem.Initialize(levelDescription.PremiumItem);
		}
	}
}
