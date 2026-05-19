using System.Collections;
using UnityEngine;

namespace Xsolla.Demo
{
	public class OwlPurchasedLevel : ILevelState
	{
		private IInventoryService InventoryService => ServiceLocator.Resolve<IInventoryService>();
		private OwlService OwlService => ServiceLocator.Resolve<OwlService>();
		private LevelStateMachine LevelStateMachine => ServiceLocator.Resolve<LevelStateMachine>();

		public void OnEnter()
		{
			CoroutineExecutor.Run(DoEnter());
		}

		private IEnumerator DoEnter()
		{
			OwlService.CheckOwlAvailability();
			Debug.Log("CONGRATS!");
			yield return null;
			LevelStateMachine.SetResumeLevel();
		}
	}
}