using System;
using System.Collections;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Core;
using Xsolla.Inventory;

namespace Xsolla.Tests
{
	public class XsollaInventoryTests
	{
		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			if (XsollaInventory.IsExist)
				XsollaInventory.DestroyImmediate(XsollaInventory.Instance.gameObject);
		}

		[UnityTest]
		public IEnumerator GetVirtualCurrencyBalance_DefaultValues_Success()
		{
			yield return GetVirtualCurrencyBalance(defaultValues: true);
		}

		[UnityTest]
		public IEnumerator GetVirtualCurrencyBalance_Parametrized_Success()
		{
			yield return GetVirtualCurrencyBalance(defaultValues: false);
		}

		[UnityTest]
		public IEnumerator GetVirtualCurrencyBalance_DefaultValues_InvalidToken_Success()
		{
			yield return TestSignInHelper.Instance.SetOldToken();
			yield return GetVirtualCurrencyBalance(defaultValues: true);
		}

		private IEnumerator GetVirtualCurrencyBalance([CallerMemberName]string testName = null, bool defaultValues = true)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			Action<VirtualCurrencyBalances> onSuccess = balances =>
			{
				if (balances == null)
				{
					errorMessage = "CONTAINER IS NULL";
					success = false;
					return;
				}

				if (balances.items == null)
				{
					errorMessage = "NESTED CONTAINER IS NULL";
					success = false;
					return;
				}

				if (balances.items.Length == 0)
				{
					errorMessage = "NESTED CONTAINER IS EMPTY";
					success = false;
					return;
				}

				success = true;
			};

			Action<Error> onError = error =>
			{
				errorMessage = error?.errorMessage ?? "ERROR IS NULL";
				success = false;
			};

			if (defaultValues)
			{
				XsollaInventory.Instance.GetVirtualCurrencyBalance(
					projectId: XsollaSettings.StoreProjectId,
					onSuccess: onSuccess,
					onError: onError);
			}
			else
			{
				XsollaInventory.Instance.GetVirtualCurrencyBalance(
					projectId: XsollaSettings.StoreProjectId,
					onSuccess: onSuccess,
					onError: onError,
					platform: "xsolla");
			}

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(testName);
			else
				TestHelper.Fail(errorMessage, testName);
		}

		[UnityTest]
		public IEnumerator GetInventoryItems_DefaultValues_Success()
		{
			yield return GetInventoryItems(defaultValues: true);
		}

		[UnityTest]
		public IEnumerator GetInventoryItems_Parametrized_Success()
		{
			yield return GetInventoryItems(defaultValues: false);
		}

		[UnityTest]
		public IEnumerator GetInventoryItems_DefaultValues_InvalidToken_Success()
		{
			yield return TestSignInHelper.Instance.SetOldToken();
			yield return GetInventoryItems(defaultValues: true);
		}

		private IEnumerator GetInventoryItems([CallerMemberName]string testName = null, bool defaultValues = true)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			Action<InventoryItems> onSuccess = items =>
			{
				if (items == null)
				{
					errorMessage = "CONTAINER IS NULL";
					success = false;
					return;
				}

				if (items.items == null)
				{
					errorMessage = "NESTED CONTAINER IS NULL";
					success = false;
					return;
				}

				if (items.items.Length == 0)
				{
					errorMessage = "NESTED CONTAINER IS EMPTY";
					success = false;
					return;
				}

				if (!defaultValues && items.items.Length != 5)
				{
					errorMessage = "LIMIT ERROR";
					success = false;
					return;
				}

				success = true;
			};

			Action<Error> onError = error =>
			{
				errorMessage = error?.errorMessage ?? "ERROR IS NULL";
				success = false;
			};

			if (defaultValues)
			{
				XsollaInventory.Instance.GetInventoryItems(
					projectId: XsollaSettings.StoreProjectId,
					onSuccess: onSuccess,
					onError: onError);
			}
			else
			{
				XsollaInventory.Instance.GetInventoryItems(
					projectId: XsollaSettings.StoreProjectId,
					onSuccess: onSuccess,
					onError: onError,
					limit: 5,
					offset: 5,
					locale: "en_US",
					platform: "xsolla");
			}

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(testName);
			else
				TestHelper.Fail(errorMessage, testName);
		}

		[UnityTest]
		public IEnumerator ConsumeInventoryItem_DefaultValues_Success()
		{
			yield return ConsumeInventoryItem(defaultValues: true);
		}

		[UnityTest]
		public IEnumerator ConsumeInventoryItem_Parametrized_Success()
		{
			yield return ConsumeInventoryItem(defaultValues: false);
		}

		[UnityTest]
		public IEnumerator ConsumeInventoryItem_DefaultValues_InvalidToken_Success()
		{
			yield return TestSignInHelper.Instance.SetOldToken();
			yield return ConsumeInventoryItem(defaultValues: true);
		}

		private IEnumerator ConsumeInventoryItem([CallerMemberName]string testName = null, bool defaultValues = true)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			Action onSuccess = () =>
			{
				success = true;
			};

			Action<Error> onError = error =>
			{
				errorMessage = error?.errorMessage ?? "ERROR IS NULL";
				success = false;
			};

			if (defaultValues)
			{
				XsollaInventory.Instance.ConsumeInventoryItem(
					projectId: XsollaSettings.StoreProjectId,
					item: new ConsumeItem() { sku = "lootbox_1", quantity = 1 },
					onSuccess: onSuccess,
					onError: onError);
			}
			else
			{
				XsollaInventory.Instance.ConsumeInventoryItem(
					projectId: XsollaSettings.StoreProjectId,
					item: new ConsumeItem() { sku = "lootbox_1", quantity = 1 },
					onSuccess: onSuccess,
					onError: onError,
					platform: "xsolla");
			}

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(testName);
			else
				TestHelper.Fail(errorMessage, testName);
		}

		[UnityTest]
		public IEnumerator GetTimeLimitedItems_DefaultValues_Success()
		{
			yield return GetTimeLimitedItems(defaultValues: true);
		}

		[UnityTest]
		public IEnumerator GetTimeLimitedItems_Parametrized_Success()
		{
			yield return GetTimeLimitedItems(defaultValues: false);
		}

		[UnityTest]
		public IEnumerator GetTimeLimitedItems_DefaultValues_InvalidToken_Success()
		{
			yield return TestSignInHelper.Instance.SetOldToken();
			yield return GetTimeLimitedItems(defaultValues: true);
		}

		private IEnumerator GetTimeLimitedItems([CallerMemberName]string testName = null, bool defaultValues = true)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			Action<TimeLimitedItems> onSuccess = items =>
			{
				if (items == null)
				{
					errorMessage = "CONTAINER IS NULL";
					success = false;
					return;
				}

				if (items.items == null)
				{
					errorMessage = "NESTED CONTAINER IS NULL";
					success = false;
					return;
				}

				if (items.items.Length == 0)
				{
					errorMessage = "NESTED CONTAINER IS EMPTY";
					success = false;
					return;
				}

				success = true;
			};

			Action<Error> onError = error =>
			{
				errorMessage = error?.errorMessage ?? "ERROR IS NULL";
				success = false;
			};

			if (defaultValues)
			{
				XsollaInventory.Instance.GetTimeLimitedItems(
					projectId: XsollaSettings.StoreProjectId,
					onSuccess: onSuccess,
					onError: onError);
			}
			else
			{
				XsollaInventory.Instance.GetTimeLimitedItems(
					projectId: XsollaSettings.StoreProjectId,
					onSuccess: onSuccess,
					onError: onError,
					platform: "xsolla");
			}

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(testName);
			else
				TestHelper.Fail(errorMessage, testName);
		}
	}
}
