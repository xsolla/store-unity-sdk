using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class BattlePassNameSetter : BaseBattlePassDescriptionSubscriber
    {
		[SerializeField] private Text BattlePassName;

		public override void OnBattlePassDescriptionArrived(BattlePassDescription battlePassDescription)
		{
			BattlePassName.text = battlePassDescription.Name;
		}
	}
}
