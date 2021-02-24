using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class BattlePassLevelBlock : MonoBehaviour
    {
		[SerializeField] private Text LevelLabel = default;
		[SerializeField] private BattlePassItemUI FreeItem = default;
		[SerializeField] private BattlePassItemUI PremiumItem = default;
		[SerializeField] private int CurrentLevelFontSize = default;
		[SerializeField] private GameObject CurrentLevelHighlight = default;

		private int _initialLevelLabelFontSize;

		private void Awake()
		{
			_initialLevelLabelFontSize = LevelLabel.fontSize;
		}

		public void Initialize(BattlePassLevelDescription levelDescription)
		{
			LevelLabel.text = levelDescription.Tier.ToString();
			FreeItem.Initialize(levelDescription.FreeItem);
			PremiumItem.Initialize(levelDescription.PremiumItem);
		}

		public void SetCurrent(bool isCurrent)
		{
			if (isCurrent)
				LevelLabel.fontSize = CurrentLevelFontSize;
			else
				LevelLabel.fontSize = _initialLevelLabelFontSize;

			CurrentLevelHighlight.SetActive(isCurrent);

			FreeItem.SetCurrent(isCurrent);
			PremiumItem.SetCurrent(isCurrent);
		}

		public void SetItemState(bool isPremium, BattlePassItemState itemState)
		{
			if (isPremium)
				PremiumItem.SetState(itemState);
			else
				FreeItem.SetState(itemState);
		}
	}
}
