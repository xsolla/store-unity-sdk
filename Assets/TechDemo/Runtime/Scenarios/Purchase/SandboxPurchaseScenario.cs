using System;
using System.Collections;
using Xsolla.Core;

namespace Xsolla.TechDemo.Scenarios
{
	public abstract class SandboxPurchaseScenario : PurchaseScenario
	{
		public override void SetUp()
		{
			base.SetUp();
			SetSandboxMode(true);
		}

		public override IEnumerator Execute(Action onSuccess, Action<Error> onError)
		{
			InitExecution(onSuccess, onError);

			yield return AuthAsDefaultUser();
			if (IsCompleted)
				yield break;

			yield return PurchaseItem();
		}
	}
}