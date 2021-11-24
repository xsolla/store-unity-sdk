using UnityEngine;

namespace Xsolla.Demo
{
	public class DescriptionToUserExpUISetterProxy : BaseBattlePassDescriptionSubscriber
	{
		[SerializeField] private BattlePassUserExpUISetter UserExpUISetter;

		public override void OnBattlePassDescriptionArrived(BattlePassDescription battlePassDescription)
		{
			UserExpUISetter.SetBattlePassLevels(battlePassDescription.Levels);
		}
	}
}
