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

		protected static IEnumerator WaitFor<T>(Action<Action<T>, Action<Error>> asyncCall, Action<T> onSuccess, Action<Error> onError)
		{
			var completed = false;

			asyncCall(
				result => {
					onSuccess(result);
					completed = true;
				},
				error => {
					onError(error);
					completed = true;
				});

			yield return new WaitUntil(() => completed);
		}

		protected static IEnumerator WaitFor(Action<Action, Action<Error>> asyncCall, Action onSuccess, Action<Error> onError)
		{
			var completed = false;

			asyncCall(
				() => {
					onSuccess();
					completed = true;
				},
				error => {
					onError(error);
					completed = true;
				});

			yield return new WaitUntil(() => completed);
		}

		protected IEnumerator RunAsync<T>(Action<Action<T>, Action<Error>> asyncCall, Action<T> onResult = null)
		{
			Error error = null;
			yield return WaitFor(asyncCall, result => onResult?.Invoke(result), e => error = e);

			if (!IsCompleted && error != null)
				TryCompleteError(error);
		}

		protected IEnumerator RunAsync(Action<Action, Action<Error>> asyncCall)
		{
			Error error = null;
			yield return WaitFor(asyncCall, () => { }, e => error = e);

			if (!IsCompleted && error != null)
				TryCompleteError(error);
		}

		protected IEnumerator VerifyWithRetry<T>(
			int attempts,
			float delaySeconds,
			Action<Action<T>, Action<Error>> request,
			Func<T, Error> validate,
			bool completeOnSuccess = true)
		{
			for (var attempt = 0; attempt < attempts; attempt++)
			{
				var value = default(T);
				Error requestError = null;

				yield return WaitFor(request, result => value = result, error => requestError = error);

				if (IsCompleted)
					yield break;

				var validationError = requestError ?? validate(value);
				if (validationError == null)
				{
					if (completeOnSuccess)
						TryCompleteSuccess();
					yield break;
				}

				if (attempt == attempts - 1)
				{
					TryCompleteError(validationError);
					yield break;
				}

				yield return new WaitForSeconds(delaySeconds);
			}
		}

		protected IEnumerator AuthAsDefaultUser()
		{
			yield return RunAsync((onSuccess, onError) =>
				XsollaAuth.SignIn(TestCredentials.DefaultUsername, TestCredentials.DefaultPassword, onSuccess, onError));
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