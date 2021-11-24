using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class BattlePassUserExpUISetter : BaseBattlePassUserStatSubscriber
	{
		[SerializeField] private Text UserExpLabel;
		[SerializeField] private Slider UserExpSlider;
		[SerializeField] private GameObject[] ObjectsToDisableOnLevelMax;

		private const string EXP_TEMPLATE = "XP {0}<color=#FFFFFFB2>/{1}</color>";
		private const string EXP_MAX = "XP MAX";

		private BattlePassLevelDescription[] _battlePassLevels;

		public void SetBattlePassLevels(BattlePassLevelDescription[] battlePassLevels)
		{
			_battlePassLevels = battlePassLevels;
		}

		public override void OnUserStatArrived(BattlePassUserStat userStat)
		{
			if (_battlePassLevels == null)
			{
				Debug.LogError("BattlePass levels must be provided beforehand. BattlePass levels are null.");
				return;
			}

			var levelIndex = userStat.Level - 1;
			var maxExp = _battlePassLevels[levelIndex].Experience;
			var userExp = userStat.Exp;
			
			if (levelIndex != _battlePassLevels.Length - 1)
				UserExpLabel.text = string.Format(EXP_TEMPLATE, userExp, maxExp);
			else
			{
				userExp = maxExp;
				UserExpLabel.text = EXP_MAX;

				foreach (var objectToDisable in ObjectsToDisableOnLevelMax)
					objectToDisable.SetActive(false);
			}

			UserExpSlider.maxValue = maxExp;
			UserExpSlider.value = userExp;
		}
	}
}
