using UnityEngine;

namespace Xsolla.Demo
{
	public class DescriptionToUserExpUISetterProxy : BaseBattlePassDescriptionSubscriber
	{
		[SerializeField] private UserExpUISetter UserExpUISetter = default;

		public override void OnBattlePassDescriptionArrived(BattlePassDescription battlePassDescription)
		{
			UserExpUISetter.SetBattlePassLevels(battlePassDescription.Levels);
		}
	}
}
