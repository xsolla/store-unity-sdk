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
		private string SettingsSnapshot { get; set; }

		protected Action OnSuccessCallback { get; private set; }
		protected Action<Error> OnErrorCallback { get; private set; }
		protected bool IsCompleted { get; private set; }

		public virtual void SetUp()
		{
			SettingsSnapshot = JsonUtility.ToJson(XsollaSettings.Instance);

			ClearAuthData();
		}

		public virtual void TearDown()
		{
			if (!string.IsNullOrEmpty(SettingsSnapshot))
				JsonUtility.FromJsonOverwrite(SettingsSnapshot, XsollaSettings.Instance);
		}

		public abstract IEnumerator Execute(Action onSuccess, Action<Error> onError);

		protected void InitExecution(Action onSuccess, Action<Error> onError)
		{
			OnSuccessCallback = onSuccess;
			OnErrorCallback = onError;
			IsCompleted = false;
		}

		protected void TryCompleteSuccess()
		{
			if (IsCompleted)
				return;

			IsCompleted = true;
			OnSuccessCallback?.Invoke();
		}

		protected void TryCompleteError(Error error)
		{
			if (IsCompleted)
				return;

			IsCompleted = true;
			OnErrorCallback?.Invoke(error);
		}

		protected WaitUntil WaitForCompletion()
		{
			return new WaitUntil(() => IsCompleted);
		}

		protected static void SetProjectId(string projectId)
		{
			XsollaSettings.StoreProjectId = projectId;
		}

		protected static void SetLoginId(string loginId)
		{
			XsollaSettings.LoginId = loginId;
		}

		protected static void SetOAuthClientId(int clientId)
		{
			XsollaSettings.OAuthClientId = clientId;
		}

		protected static void SetSandboxMode(bool isSandbox)
		{
			XsollaSettings.IsSandbox = isSandbox;
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