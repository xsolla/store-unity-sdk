using System.Collections;
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
		public void TearDown()
		{
			if (XsollaInventory.IsExist)
				UnityEngine.Object.DestroyImmediate(XsollaInventory.Instance.gameObject);
		}

		[UnityTest]
		public IEnumerator GetVirtualCurrencyBalance_NoPlatform_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaInventory.Instance.GetVirtualCurrencyBalance(
				projectId: XsollaSettings.StoreProjectId,
				onSuccess: balances =>
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
						errorMessage = "NESTED CONTAIER IS EMPTY";
						success = false;
						return;
					}

					success = true;
				},
				onError: error =>
				{
					errorMessage = error?.errorMessage ?? "ERROR IS NULL";
					success = false;
				});

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(nameof(GetVirtualCurrencyBalance_NoPlatform_Success));
			else
				TestHelper.Fail(nameof(GetVirtualCurrencyBalance_NoPlatform_Success), errorMessage);
		}

		[UnityTest]
		public IEnumerator GetVirtualCurrencyBalance_XsollaPlatform_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaInventory.Instance.GetVirtualCurrencyBalance(
				projectId: XsollaSettings.StoreProjectId,
				onSuccess: balances =>
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
						errorMessage = "NESTED CONTAIER IS EMPTY";
						success = false;
						return;
					}

					success = true;
				},
				onError: error =>
				{
					errorMessage = error?.errorMessage ?? "ERROR IS NULL";
					success = false;
				},
				platform: "xsolla");

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(nameof(GetVirtualCurrencyBalance_XsollaPlatform_Success));
			else
				TestHelper.Fail(nameof(GetVirtualCurrencyBalance_XsollaPlatform_Success), errorMessage);
		}

		[UnityTest]
		public IEnumerator GetVirtualCurrencyBalance_NoPlatform_InvalidToken_SuccessAndTokenRefreshed()
		{
			yield return TestSignInHelper.Instance.SetOldToken();

			bool? success = default;
			string errorMessage = default;

			XsollaInventory.Instance.GetVirtualCurrencyBalance(
				projectId: XsollaSettings.StoreProjectId,
				onSuccess: balances =>
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
						errorMessage = "NESTED CONTAIER IS EMPTY";
						success = false;
						return;
					}

					success = true;
				},
				onError: error =>
				{
					errorMessage = error?.errorMessage ?? "ERROR IS NULL";
					success = false;
				});

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.CheckTokenChanged(nameof(GetVirtualCurrencyBalance_NoPlatform_InvalidToken_SuccessAndTokenRefreshed));
			else
				TestHelper.Fail(nameof(GetVirtualCurrencyBalance_NoPlatform_InvalidToken_SuccessAndTokenRefreshed), errorMessage);
		}

		[UnityTest]
		public IEnumerator GetInventoryItems_DefaultValues_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaInventory.Instance.GetInventoryItems(
				projectId: XsollaSettings.StoreProjectId,
				onSuccess: items =>
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
						errorMessage = "NESTED CONTAIER IS EMPTY";
						success = false;
						return;
					}

					success = true;
				},
				onError: error =>
				{
					errorMessage = error?.errorMessage ?? "ERROR IS NULL";
					success = false;
				});

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(nameof(GetInventoryItems_DefaultValues_Success));
			else
				TestHelper.Fail(nameof(GetInventoryItems_DefaultValues_Success), errorMessage);
		}

		[UnityTest]
		public IEnumerator GetInventoryItems_Parametrized_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaInventory.Instance.GetInventoryItems(
				projectId: XsollaSettings.StoreProjectId,
				onSuccess: items =>
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
						errorMessage = "NESTED CONTAIER IS EMPTY";
						success = false;
						return;
					}

					if (items.items.Length != 5)
					{
						errorMessage = "LIMIT ERROR";
						success = false;
						return;
					}

					success = true;
				},
				onError: error =>
				{
					errorMessage = error?.errorMessage ?? "ERROR IS NULL";
					success = false;
				},
				limit: 5,
				offset: 5,
				locale: "en_US",
				platform: "xsolla");

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(nameof(GetInventoryItems_Parametrized_Success));
			else
				TestHelper.Fail(nameof(GetInventoryItems_Parametrized_Success), errorMessage);
		}

		[UnityTest]
		public IEnumerator GetInventoryItems_DefaultValues_InvalidToken_SuccessAndTokenRefreshed()
		{
			yield return TestSignInHelper.Instance.SetOldToken();

			bool? success = default;
			string errorMessage = default;

			XsollaInventory.Instance.GetInventoryItems(
				projectId: XsollaSettings.StoreProjectId,
				onSuccess: items =>
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
						errorMessage = "NESTED CONTAIER IS EMPTY";
						success = false;
						return;
					}

					success = true;
				},
				onError: error =>
				{
					errorMessage = error?.errorMessage ?? "ERROR IS NULL";
					success = false;
				});

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.CheckTokenChanged(nameof(GetInventoryItems_DefaultValues_InvalidToken_SuccessAndTokenRefreshed));
			else
				TestHelper.Fail(nameof(GetInventoryItems_DefaultValues_InvalidToken_SuccessAndTokenRefreshed), errorMessage);
		}

		[UnityTest]
		public IEnumerator ConsumeInventoryItem_NoPlatform_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaInventory.Instance.ConsumeInventoryItem(
				projectId: XsollaSettings.StoreProjectId,
				item: new ConsumeItem(){sku="lootbox_1", quantity=1 },
				onSuccess: () =>
				{
					success = true;
				},
				onError: error =>
				{
					errorMessage = error?.errorMessage ?? "ERROR IS NULL";
					success = false;
				});

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(nameof(ConsumeInventoryItem_NoPlatform_Success));
			else
				TestHelper.Fail(nameof(ConsumeInventoryItem_NoPlatform_Success), errorMessage);
		}

		[UnityTest]
		public IEnumerator ConsumeInventoryItem_XsollaPlatform_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaInventory.Instance.ConsumeInventoryItem(
				projectId: XsollaSettings.StoreProjectId,
				item: new ConsumeItem() { sku = "lootbox_1", quantity = 1 },
				onSuccess: () =>
				{
					success = true;
				},
				onError: error =>
				{
					errorMessage = error?.errorMessage ?? "ERROR IS NULL";
					success = false;
				},
				platform:"xsolla");

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(nameof(ConsumeInventoryItem_XsollaPlatform_Success));
			else
				TestHelper.Fail(nameof(ConsumeInventoryItem_XsollaPlatform_Success), errorMessage);
		}

		[UnityTest]
		public IEnumerator ConsumeInventoryItem_NoPlatform_InvalidToken_SuccessAndTokenRefreshed()
		{
			yield return TestSignInHelper.Instance.SetOldToken();

			bool? success = default;
			string errorMessage = default;

			XsollaInventory.Instance.ConsumeInventoryItem(
				projectId: XsollaSettings.StoreProjectId,
				item: new ConsumeItem() { sku = "lootbox_1", quantity = 1 },
				onSuccess: () =>
				{
					success = true;
				},
				onError: error =>
				{
					errorMessage = error?.errorMessage ?? "ERROR IS NULL";
					success = false;
				});

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.CheckTokenChanged(nameof(ConsumeInventoryItem_NoPlatform_InvalidToken_SuccessAndTokenRefreshed));
			else
				TestHelper.Fail(nameof(ConsumeInventoryItem_NoPlatform_InvalidToken_SuccessAndTokenRefreshed), errorMessage);
		}
	}
}
