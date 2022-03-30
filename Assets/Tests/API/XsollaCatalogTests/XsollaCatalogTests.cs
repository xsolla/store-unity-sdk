using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.Tests
{
	public class XsollaCatalogTests
    {
		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			if (XsollaCatalog.IsExist)
				XsollaCatalog.DestroyImmediate(XsollaCatalog.Instance.gameObject);
		}

		[UnityTest]
        public IEnumerator GetCatalog_DefaultValues_Success()
        {
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaCatalog.Instance.GetCatalog(
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
				TestHelper.Pass(nameof(GetCatalog_DefaultValues_Success));
			else
				TestHelper.Fail(nameof(GetCatalog_DefaultValues_Success), errorMessage);
		}

		[UnityTest]
		public IEnumerator GetCatalog_Parametrized_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaCatalog.Instance.GetCatalog(
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

					if (items.items.Length != 10)
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
				limit: 10,
				offset: 10,
				locale: "en_US",
				additionalFields: null,
				country: "US");

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(nameof(GetCatalog_Parametrized_Success));
			else
				TestHelper.Fail(nameof(GetCatalog_Parametrized_Success), errorMessage);
		}

		[UnityTest]
		public IEnumerator GetCatalogSimplified_DefaultValues_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaCatalog.Instance.GetCatalogSimplified(
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
				TestHelper.Pass(nameof(GetCatalogSimplified_DefaultValues_Success));
			else
				TestHelper.Fail(nameof(GetCatalogSimplified_DefaultValues_Success), errorMessage);
		}

		[UnityTest]
		public IEnumerator GetCatalogSimplified_Parametrized_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaCatalog.Instance.GetCatalogSimplified(
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
				},
				locale: "en_US");

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(nameof(GetCatalogSimplified_Parametrized_Success));
			else
				TestHelper.Fail(nameof(GetCatalogSimplified_Parametrized_Success), errorMessage);
		}

		[UnityTest]
		public IEnumerator GetGroupItems_DefaultValues_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaCatalog.Instance.GetGroupItems(
				projectId: XsollaSettings.StoreProjectId,
				groupExternalId: "Featured",
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
				TestHelper.Pass(nameof(GetGroupItems_DefaultValues_Success));
			else
				TestHelper.Fail(nameof(GetGroupItems_DefaultValues_Success), errorMessage);
		}

		[UnityTest]
		public IEnumerator GetGroupItems_Parametrized_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaCatalog.Instance.GetGroupItems(
				projectId: XsollaSettings.StoreProjectId,
				groupExternalId: "Featured",
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
				additionalFields: "long_description",
				country: "US");

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(nameof(GetGroupItems_Parametrized_Success));
			else
				TestHelper.Fail(nameof(GetGroupItems_Parametrized_Success), errorMessage);
		}

		[UnityTest]
		public IEnumerator GetItemGroups_DefaultValues_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaCatalog.Instance.GetItemGroups(
				projectId: XsollaSettings.StoreProjectId,
				onSuccess: groups =>
				{
					if (groups == null)
					{
						errorMessage = "CONTAINER IS NULL";
						success = false;
						return;
					}

					if (groups.groups == null)
					{
						errorMessage = "NESTED CONTAINER IS NULL";
						success = false;
						return;
					}

					if (groups.groups.Length == 0)
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
				TestHelper.Pass(nameof(GetItemGroups_DefaultValues_Success));
			else
				TestHelper.Fail(nameof(GetItemGroups_DefaultValues_Success), errorMessage);
		}

		[UnityTest]
		public IEnumerator GetItemGroups_Parametrized_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaCatalog.Instance.GetItemGroups(
				projectId: XsollaSettings.StoreProjectId,
				onSuccess: groups =>
				{
					if (groups == null)
					{
						errorMessage = "CONTAINER IS NULL";
						success = false;
						return;
					}

					if (groups.groups == null)
					{
						errorMessage = "NESTED CONTAINER IS NULL";
						success = false;
						return;
					}

					if (groups.groups.Length == 0)
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
				limit: 2,
				offset: 1,
				locale: "en_US");

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(nameof(GetItemGroups_Parametrized_Success));
			else
				TestHelper.Fail(nameof(GetItemGroups_Parametrized_Success), errorMessage);
		}

		[UnityTest]
		public IEnumerator GetVirtualCurrencyList_DefaultValues_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaCatalog.Instance.GetVirtualCurrencyList(
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
				TestHelper.Pass(nameof(GetVirtualCurrencyList_DefaultValues_Success));
			else
				TestHelper.Fail(nameof(GetVirtualCurrencyList_DefaultValues_Success), errorMessage);
		}

		[UnityTest]
		public IEnumerator GetVirtualCurrencyList_Parametrized_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaCatalog.Instance.GetVirtualCurrencyList(
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

					if (items.items.Length != 1)
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
				limit: 1,
				offset: 1,
				locale: "en_US",
				additionalFields: "long_description",
				country: "US");

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(nameof(GetVirtualCurrencyList_Parametrized_Success));
			else
				TestHelper.Fail(nameof(GetVirtualCurrencyList_Parametrized_Success), errorMessage);
		}

		[UnityTest]
		public IEnumerator GetVirtualCurrencyPackagesList_DefaultValues_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaCatalog.Instance.GetVirtualCurrencyPackagesList(
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

					if (items.items.Count == 0)
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
				TestHelper.Pass(nameof(GetVirtualCurrencyPackagesList_DefaultValues_Success));
			else
				TestHelper.Fail(nameof(GetVirtualCurrencyPackagesList_DefaultValues_Success), errorMessage);
		}

		[UnityTest]
		public IEnumerator GetVirtualCurrencyPackagesList_Parametrized_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaCatalog.Instance.GetVirtualCurrencyPackagesList(
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

					if (items.items.Count == 0)
					{
						errorMessage = "NESTED CONTAIER IS EMPTY";
						success = false;
						return;
					}

					if (items.items.Count != 1)
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
				limit: 1,
				offset: 1,
				locale: "en_US",
				additionalFields: "long_description",
				country: "US");

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(nameof(GetVirtualCurrencyPackagesList_Parametrized_Success));
			else
				TestHelper.Fail(nameof(GetVirtualCurrencyPackagesList_Parametrized_Success), errorMessage);
		}

		[UnityTest]
		public IEnumerator GetBundles_DefaultValues_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaCatalog.Instance.GetBundles(
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
				TestHelper.Pass(nameof(GetBundles_DefaultValues_Success));
			else
				TestHelper.Fail(nameof(GetBundles_DefaultValues_Success), errorMessage);
		}

		[UnityTest]
		public IEnumerator GetBundles_Parametrized_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaCatalog.Instance.GetBundles(
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

					if (items.items.Length != 1)
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
				limit: 1,
				offset: 1,
				locale: "en_US",
				additionalFields: "long_description",
				country: "US");

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(nameof(GetBundles_Parametrized_Success));
			else
				TestHelper.Fail(nameof(GetBundles_Parametrized_Success), errorMessage);
		}

		[UnityTest]
		public IEnumerator GetBundle_DefaultValues_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaCatalog.Instance.GetBundle(
				projectId: XsollaSettings.StoreProjectId,
				sku: "starter_pack",
				onSuccess: bundle =>
				{
					if (bundle == null)
					{
						errorMessage = "BUNDLE IS NULL";
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
				TestHelper.Pass(nameof(GetBundle_DefaultValues_Success));
			else
				TestHelper.Fail(nameof(GetBundle_DefaultValues_Success), errorMessage);
		}

		[UnityTest]
		public IEnumerator GetBundle_Parametrized_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaCatalog.Instance.GetBundle(
				projectId: XsollaSettings.StoreProjectId,
				sku: "starter_pack",
				onSuccess: bundle =>
				{
					if (bundle == null)
					{
						errorMessage = "CONTAINER IS NULL";
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
				locale: "en_US",
				country: "US");

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(nameof(GetBundle_Parametrized_Success));
			else
				TestHelper.Fail(nameof(GetBundle_Parametrized_Success), errorMessage);
		}

		[UnityTest]
		public IEnumerator RedeemCouponCode_WINTER2021_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaCatalog.Instance.RedeemCouponCode(
				projectId: XsollaSettings.StoreProjectId,
				couponCode: new CouponCode() { coupon_code = "WINTER2021" },
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
				TestHelper.Pass(nameof(RedeemCouponCode_WINTER2021_Success));
			else
				TestHelper.Fail(nameof(RedeemCouponCode_WINTER2021_Success), errorMessage);
		}

		[UnityTest]
		public IEnumerator RedeemCouponCode_IncorrectCode_Failure()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaCatalog.Instance.RedeemCouponCode(
				projectId: XsollaSettings.StoreProjectId,
				couponCode: new CouponCode() { coupon_code = "SUMMER1967" },
				onSuccess: _ =>
				{
					success = true;
				},
				onError: error =>
				{
					errorMessage = error?.errorMessage ?? "ERROR IS NULL";
					success = false;
				});

			yield return new WaitUntil(() => success.HasValue);

			if (!success.Value)
				TestHelper.Pass(nameof(RedeemCouponCode_IncorrectCode_Failure), errorMessage);
			else
				TestHelper.Fail(nameof(RedeemCouponCode_IncorrectCode_Failure));
		}

		[UnityTest]
		public IEnumerator RedeemCouponCode_WINTER2021_InvalidToken_SuccessAndTokenRefreshed()
		{
			yield return TestSignInHelper.Instance.SetOldToken();

			bool? success = default;
			string errorMessage = default;

			XsollaCatalog.Instance.RedeemCouponCode(
				projectId: XsollaSettings.StoreProjectId,
				couponCode: new CouponCode() { coupon_code = "WINTER2021" },
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
				TestHelper.CheckTokenChanged(nameof(RedeemCouponCode_WINTER2021_InvalidToken_SuccessAndTokenRefreshed));
			else
				TestHelper.Fail(nameof(RedeemCouponCode_WINTER2021_InvalidToken_SuccessAndTokenRefreshed), errorMessage);
		}

		[UnityTest]
		public IEnumerator GetCouponRewards_WINTER2021_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaCatalog.Instance.GetCouponRewards(
				projectId: XsollaSettings.StoreProjectId,
				couponCode: "WINTER2021",
				onSuccess: bonus =>
				{
					if (bonus == null)
					{
						errorMessage = "CONTAINER IS NULL";
						success = false;
						return;
					}

					if (bonus.bonus == null)
					{
						errorMessage = "NESTED CONTAINER IS NULL";
						success = false;
						return;
					}

					if (bonus.bonus.Length == 0)
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
				TestHelper.Pass(nameof(GetCouponRewards_WINTER2021_Success));
			else
				TestHelper.Fail(nameof(GetCouponRewards_WINTER2021_Success), errorMessage);
		}

		[UnityTest]
		public IEnumerator GetCouponRewards_IncorrectCode_Failure()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaCatalog.Instance.GetCouponRewards(
				projectId: XsollaSettings.StoreProjectId,
				couponCode: "SUMMER1967",
				onSuccess: _ =>
				{
					success = true;
				},
				onError: error =>
				{
					errorMessage = error?.errorMessage ?? "ERROR IS NULL";
					success = false;
				});

			yield return new WaitUntil(() => success.HasValue);

			if (!success.Value)
				TestHelper.Pass(nameof(GetCouponRewards_IncorrectCode_Failure), errorMessage);
			else
				TestHelper.Fail(nameof(GetCouponRewards_IncorrectCode_Failure));
		}

		[UnityTest]
		public IEnumerator GetCouponRewards_WINTER2021_InvalidToken_SuccessAndTokenRefreshed()
		{
			yield return TestSignInHelper.Instance.SetOldToken();

			bool? success = default;
			string errorMessage = default;

			XsollaCatalog.Instance.GetCouponRewards(
				projectId: XsollaSettings.StoreProjectId,
				couponCode: "WINTER2021",
				onSuccess: bonus =>
				{
					if (bonus == null)
					{
						errorMessage = "CONTAINER IS NULL";
						success = false;
						return;
					}

					if (bonus.bonus == null)
					{
						errorMessage = "NESTED CONTAINER IS NULL";
						success = false;
						return;
					}

					if (bonus.bonus.Length == 0)
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
				TestHelper.CheckTokenChanged(nameof(GetCouponRewards_WINTER2021_InvalidToken_SuccessAndTokenRefreshed));
			else
				TestHelper.Fail(nameof(GetCouponRewards_WINTER2021_InvalidToken_SuccessAndTokenRefreshed), errorMessage);
		}
	}
}
