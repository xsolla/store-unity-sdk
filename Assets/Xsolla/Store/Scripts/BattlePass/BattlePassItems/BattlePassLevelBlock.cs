using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class BattlePassLevelBlock : MonoBehaviour
    {
		[SerializeField] private BattlePassItem freeItem;
		[SerializeField] private BattlePassItem premiumItem;

		[SerializeField] private Text LevelLabel;
		[SerializeField] private int CurrentLevelFontSize;
		[SerializeField] private GameObject CurrentLevelHighlight;

		private int _initialLevelLabelFontSize;

		public BattlePassItem FreeItem
		{
			get
			{
				return freeItem;
			}
		}
		public BattlePassItem PremiumItem
		{
			get
			{
				return premiumItem;
			}
		}

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
