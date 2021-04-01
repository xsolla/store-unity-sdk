using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class BattlePassLevelBlock : MonoBehaviour
    {
		[SerializeField] private BattlePassItem freeItem = default;
		[SerializeField] private BattlePassItem premiumItem = default;

		[SerializeField] private Text LevelLabel = default;
		[SerializeField] private int CurrentLevelFontSize = default;
		[SerializeField] private GameObject CurrentLevelHighlight = default;

		private int _initialLevelLabelFontSize;

		public BattlePassItem FreeItem => freeItem;
		public BattlePassItem PremiumItem => premiumItem;

		private void Awake()
		{
			_initialLevelLabelFontSize = LevelLabel.fontSize;
		}

		public void Initialize(BattlePassLevelDescription levelDescription)
		{
			LevelLabel.text = levelDescription.Tier.ToString();
			freeItem.Initialize(levelDescription.FreeItem);
			premiumItem.Initialize(levelDescription.PremiumItem);
		}

		public void SetCurrent(bool isCurrent)
		{
			if (isCurrent)
				LevelLabel.fontSize = CurrentLevelFontSize;
			else
				LevelLabel.fontSize = _initialLevelLabelFontSize;

			CurrentLevelHighlight.SetActive(isCurrent);

			freeItem.SetCurrent(isCurrent);
			premiumItem.SetCurrent(isCurrent);
		}

		public void ShowLevelLabel(bool show)
		{
			LevelLabel.gameObject.SetActive(show);
		}
	}
}
