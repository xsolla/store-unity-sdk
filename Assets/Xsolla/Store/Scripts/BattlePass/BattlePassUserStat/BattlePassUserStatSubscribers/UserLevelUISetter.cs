using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class UserLevelUISetter : BaseUserStatSubscriber
	{
		[SerializeField] private Text[] UserLevelLabels = default;

		public override void OnUserStatArrived(BattlePassUserStat userStat)
		{
			foreach (var label in UserLevelLabels)
				label.text = userStat.Level.ToString();
		}
	}
}
