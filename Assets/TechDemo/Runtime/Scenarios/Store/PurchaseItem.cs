using System;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.TechDemo.Scenarios
{
	public class PurchaseItem : Scenario
	{
		public override void Execute(Action onSuccess, Action<Error> onError)
		{
			ClearAuthData();

			AuthAsDemoUser(
				() => OnAuthSuccess(onSuccess, onError),
				onError.Invoke);
		}

		private static void OnAuthSuccess(Action onSuccess, Action<Error> onError)
		{
			XsollaCatalog.Purchase(
				"booster_max_1",
				_ => onSuccess.Invoke(),
				onError);
		}
	}
}