using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class BattlePassExpBarColorSetter : BaseBattlePassDescriptionSubscriber
	{
		[SerializeField] private Image ExpBar;
		[SerializeField] private Color ExpiredColor;

		public override void OnBattlePassDescriptionArrived(BattlePassDescription battlePassDescription)
		{
			if (battlePassDescription.IsExpired)
				ExpBar.color = ExpiredColor;
		}
	}
}
