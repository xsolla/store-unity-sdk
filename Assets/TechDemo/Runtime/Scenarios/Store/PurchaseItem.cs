using System;
using System.Collections;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.TechDemo.Scenarios
{
	public class PurchaseItem : Scenario
	{
		public override IEnumerator Execute(Action onSuccess, Action<Error> onError)
		{
			ClearAuthData();

			yield break;
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