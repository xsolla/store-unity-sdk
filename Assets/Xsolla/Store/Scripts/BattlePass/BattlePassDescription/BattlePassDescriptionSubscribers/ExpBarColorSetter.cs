using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class ExpBarColorSetter : BaseBattlePassDescriptionSubscriber
	{
		[SerializeField] private Image ExpBar = default;
		[SerializeField] private Color ExpiredColor = default;

		public override void OnBattlePassDescriptionArrived(BattlePassDescription battlePassDescription)
		{
			if (battlePassDescription.IsExpired)
				ExpBar.color = ExpiredColor;
		}
	}
}
