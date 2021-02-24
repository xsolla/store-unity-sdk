using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassItemsSetter : BaseBattlePassDescriptionSubscriber
	{
		[SerializeField] private GameObject LevelBlockPrefab = default;
		[SerializeField] private Transform ItemsRoot = default;

		public override void OnBattlePassDescriptionArrived(BattlePassDescription battlePassDescription)
		{
			foreach (var levelDescription in battlePassDescription.Levels)
			{
				var levelGameObject = Object.Instantiate(LevelBlockPrefab, ItemsRoot);
				var levelBlockscript = levelGameObject.GetComponent<BattlePassLevelBlock>();
				levelBlockscript.Initialize(levelDescription);
			}
		}
	}
}
