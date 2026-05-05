using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Xsolla.Auth;
using Xsolla.Core;
using Xsolla.Inventory;
using Xsolla.Tests;

namespace Xsolla.TechDemo.Scenarios
{
	public abstract class Scenario
	{
		public virtual void SetUp()
		{
			ClearAuthData();
		}

		public abstract IEnumerator Execute(Action onSuccess, Action<Error> onError);

		public virtual void TearDown()
		{
		}

		protected static void SetInAppBrowserActive(bool isActive)
		{
			XsollaSettings.InAppBrowserEnabled = isActive;
		}

		protected static void CloseInAppBrowser()
		{
			XsollaWebBrowser.Close();
		}

		protected static void ClearAuthData()
		{
			XsollaToken.DeleteSavedInstance();
		}

		protected static IEnumerator AuthAsDefaultUser(Action<Error> onError)
		{
			var username = TestCredentials.DefaultUsername;
			var password = TestCredentials.DefaultPassword;

			var isComplete = false;

			XsollaAuth.SignIn(
				username,
				password,
				() => isComplete = true,
				error => {
					isComplete = true;
					onError(error);
				});

			yield return new WaitUntil(() => isComplete);
		}

		protected static void GetVirtualCurrencyBalance(string sku, Action<int> onSuccess, Action<Error> onError)
		{
			XsollaInventory.GetInventoryItems(
				items => {
					var item = items.items.FirstOrDefault(x => x.sku == sku);
					if (item == null)
					{
						onError?.Invoke(new Error(errorMessage: $"Item with sku '{sku}' not found in inventory"));
						return;
					}

					var quantity = item.quantity ?? 0;
					onSuccess?.Invoke(quantity);
				},
				error => onError?.Invoke(error));
		}
	}
}