using System;
using System.Collections;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.TechDemo.Scenarios
{
	public abstract class LivePurchaseScenario : PurchaseScenario
	{
		private const string ProjectId = "185446";

		public override void SetUp()
		{
			base.SetUp();
			SetSandboxMode(false);
			SetProjectId(ProjectId);
		}

		public override IEnumerator Execute(Action onSuccess, Action<Error> onError)
		{
			InitExecution(onSuccess, onError);

			yield return WaitFor(
				(widgetSuccess, widgetError) => XsollaAuth.AuthWithXsollaWidget(
					widgetSuccess,
					widgetError,
					() => widgetError(new Error(errorMessage: "Canceled by user"))),
				() => { },
				TryCompleteError);

			if (IsCompleted)
				yield break;

			yield return PurchaseItem();
		}
	}
}