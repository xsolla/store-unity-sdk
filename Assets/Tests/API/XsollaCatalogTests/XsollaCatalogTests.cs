using System;
using System.Collections;
using System.Runtime.CompilerServices;
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
			yield return GetCatalog(defaultValues: true);
		}

		[UnityTest]
		public IEnumerator GetCatalog_Parametrized_Success()
		{
			yield return GetCatalog(defaultValues: false);
		}

		private IEnumerator GetCatalog([CallerMemberName]string testName = null, bool defaultValues = true)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			Action<StoreItems> onSuccess = items =>
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

				if (!defaultValues && items.items.Length != 10)
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
				XsollaCatalog.Instance.GetCatalog(
					projectId: XsollaSettings.StoreProjectId,
					onSuccess: onSuccess,
					onError: onError);
			}
			else
			{
				XsollaCatalog.Instance.GetCatalog(
					projectId: XsollaSettings.StoreProjectId,
					onSuccess: onSuccess,
					onError: onError,
					limit: 10,
					offset: 10,
					locale: "en_US",
					additionalFields: null,
					country: "US");
			}

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(testName);
			else
				TestHelper.Fail(errorMessage, testName);
		}

		[UnityTest]
		public IEnumerator GetCatalogSimplified_DefaultValues_Success()
		{
			yield return GetCatalogSmplified(defaultValues: true);
		}

		[UnityTest]
		public IEnumerator GetCatalogSimplified_Parametrized_Success()
		{
			yield return GetCatalogSmplified(defaultValues: false);
		}

		private IEnumerator GetCatalogSmplified([CallerMemberName]string testName = null, bool defaultValues = true)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			Action<StoreItemShortCollection> onSuccess = items =>
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
				XsollaCatalog.Instance.GetCatalogSimplified(
					projectId: XsollaSettings.StoreProjectId,
					onSuccess: onSuccess,
					onError: onError);
			}
			else
			{
				XsollaCatalog.Instance.GetCatalogSimplified(
					projectId: XsollaSettings.StoreProjectId,
					onSuccess: onSuccess,
					onError: onError,
					locale: "de_DE");
			}

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(testName);
			else
				TestHelper.Fail(errorMessage, testName);
		}

		[UnityTest]
		public IEnumerator GetGroupItems_DefaultValues_Success()
		{
			yield return GetGroupItems(defaultValues: true);
		}

		[UnityTest]
		public IEnumerator GetGroupItems_Parametrized_Success()
		{
			yield return GetGroupItems(defaultValues: false);
		}

		private IEnumerator GetGroupItems([CallerMemberName]string testName = null, bool defaultValues = true)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			Action<StoreItems> onSuccess = items =>
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
				XsollaCatalog.Instance.GetGroupItems(
					projectId: XsollaSettings.StoreProjectId,
					groupExternalId: "Featured",
					onSuccess: onSuccess,
					onError: onError);
			}
			else
			{
				XsollaCatalog.Instance.GetGroupItems(
					projectId: XsollaSettings.StoreProjectId,
					groupExternalId: "Featured",
					onSuccess: onSuccess,
					onError: onError,
					limit: 5,
					offset: 5,
					locale: "en_US",
					additionalFields: "long_description",
					country: "US");
			}

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(testName);
			else
				TestHelper.Fail(errorMessage, testName);
		}

		[UnityTest]
		public IEnumerator GetItemGroups_DefaultValues_Success()
		{
			yield return GetItemGroups(defaultValues: true);
		}

		[UnityTest]
		public IEnumerator GetItemGroups_Parametrized_Success()
		{
			yield return GetItemGroups(defaultValues: false);
		}

		private IEnumerator GetItemGroups([CallerMemberName]string testName = null, bool defaultValues = true)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			Action<Groups> onSuccess = groups =>
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
				XsollaCatalog.Instance.GetItemGroups(
					projectId: XsollaSettings.StoreProjectId,
					onSuccess: onSuccess,
					onError: onError);
			}
			else
			{
				XsollaCatalog.Instance.GetItemGroups(
					projectId: XsollaSettings.StoreProjectId,
					onSuccess: onSuccess,
					onError: onError,
					limit: 2,
					offset: 1,
					locale: "en_US");
			}

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(testName);
			else
				TestHelper.Fail(errorMessage, testName);
		}

		[UnityTest]
		public IEnumerator GetVirtualCurrencyList_DefaultValues_Success()
		{
			yield return GetVirtualCurrencyList(defaultValues: true);
		}

		[UnityTest]
		public IEnumerator GetVirtualCurrencyList_Parametrized_Success()
		{
			yield return GetVirtualCurrencyList(defaultValues: false);
		}

		private IEnumerator GetVirtualCurrencyList([CallerMemberName]string testName = null, bool defaultValues = true)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			Action<VirtualCurrencyItems> onSuccess = items =>
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

				if (!defaultValues && items.items.Length != 1)
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
				XsollaCatalog.Instance.GetVirtualCurrencyList(
					projectId: XsollaSettings.StoreProjectId,
					onSuccess: onSuccess,
					onError: onError);
			}
			else
			{
				XsollaCatalog.Instance.GetVirtualCurrencyList(
					projectId: XsollaSettings.StoreProjectId,
					onSuccess: onSuccess,
					onError: onError,
					limit: 1,
					offset: 1,
					locale: "en_US",
					additionalFields: "long_description",
					country: "US");
			}

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(testName);
			else
				TestHelper.Fail(errorMessage, testName);
		}

		[UnityTest]
		public IEnumerator GetVirtualCurrencyPackagesList_DefaultValues_Success()
		{
			yield return GetVirtualCurrencyPackagesList(defaultValues: true);
		}

		[UnityTest]
		public IEnumerator GetVirtualCurrencyPackagesList_Parametrized_Success()
		{
			yield return GetVirtualCurrencyPackagesList(defaultValues: false);
		}

		private IEnumerator GetVirtualCurrencyPackagesList([CallerMemberName]string testName = null, bool defaultValues = true)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			Action<VirtualCurrencyPackages> onSuccess = items =>
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
					errorMessage = "NESTED CONTAINER IS EMPTY";
					success = false;
					return;
				}

				if (!defaultValues && items.items.Count != 1)
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
				XsollaCatalog.Instance.GetVirtualCurrencyPackagesList(
					projectId: XsollaSettings.StoreProjectId,
					onSuccess: onSuccess,
					onError: onError);
			}
			else
			{
				XsollaCatalog.Instance.GetVirtualCurrencyPackagesList(
					projectId: XsollaSettings.StoreProjectId,
					onSuccess: onSuccess,
					onError: onError,
					limit: 1,
					offset: 1,
					locale: "en_US",
					additionalFields: "long_description",
					country: "US");
			}

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(testName);
			else
				TestHelper.Fail(errorMessage, testName);
		}

		[UnityTest]
		public IEnumerator GetBundles_DefaultValues_Success()
		{
			yield return GetBundles(defaultValues: true);
		}

		[UnityTest]
		public IEnumerator GetBundles_Parametrized_Success()
		{
			yield return GetBundles(defaultValues: false);
		}

		private IEnumerator GetBundles([CallerMemberName]string testName = null, bool defaultValues = true)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			Action<BundleItems> onSuccess = items =>
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

				if (!defaultValues && items.items.Length != 1)
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
				XsollaCatalog.Instance.GetBundles(
					projectId: XsollaSettings.StoreProjectId,
					onSuccess: onSuccess,
					onError: onError);
			}
			else
			{
				XsollaCatalog.Instance.GetBundles(
					projectId: XsollaSettings.StoreProjectId,
					onSuccess: onSuccess,
					onError: onError,
					limit: 1,
					offset: 1,
					locale: "en_US",
					additionalFields: "long_description",
					country: "US");
			}

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(testName);
			else
				TestHelper.Fail(errorMessage, testName);
		}

		[UnityTest]
		public IEnumerator GetBundle_DefaultValues_Success()
		{
			yield return GetBundle(defaultValues: true);
		}

		[UnityTest]
		public IEnumerator GetBundle_Parametrized_Success()
		{
			yield return GetBundle(defaultValues: false);
		}

		private IEnumerator GetBundle([CallerMemberName]string testName = null, bool defaultValues = true)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			Action<BundleItem> onSuccess = bundle =>
			{
				if (bundle == null)
				{
					errorMessage = "CONTAINER IS NULL";
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
				XsollaCatalog.Instance.GetBundle(
					projectId: XsollaSettings.StoreProjectId,
					sku: "starter_pack",
					onSuccess: onSuccess,
					onError: onError);
			}
			else
			{
				XsollaCatalog.Instance.GetBundle(
					projectId: XsollaSettings.StoreProjectId,
					sku: "starter_pack",
					onSuccess: onSuccess,
					onError: onError,
					locale: "en_US",
					country: "US");
			}

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(testName);
			else
				TestHelper.Fail(errorMessage, testName);
		}

		[UnityTest]
		public IEnumerator RedeemCouponCode_TEST2022_Success()
		{
			yield return RedeemCouponCode(couponCode: "TEST2022", isSuccessExpected: true);
		}

		[UnityTest]
		public IEnumerator RedeemCouponCode_IncorrectCode_Failure()
		{
			yield return RedeemCouponCode(couponCode: "SUMMER1967", isSuccessExpected: false);
		}

		[UnityTest]
		public IEnumerator RedeemCouponCode_TEST2022_InvalidToken_Success()
		{
			yield return TestSignInHelper.Instance.SetOldToken();
			yield return RedeemCouponCode(couponCode: "TEST2022", isSuccessExpected: true);
		}

		private IEnumerator RedeemCouponCode([CallerMemberName]string testName = null, string couponCode = null, bool isSuccessExpected = true)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			Action<CouponRedeemedItems> onSuccess = items =>
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

			XsollaCatalog.Instance.RedeemCouponCode(
				projectId: XsollaSettings.StoreProjectId,
				couponCode: new CouponCode() { coupon_code = couponCode },
				onSuccess: onSuccess,
				onError: onError);

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value == isSuccessExpected)
				TestHelper.Pass(errorMessage, testName);
			else
				TestHelper.Fail(errorMessage, testName);
		}

		[UnityTest]
		public IEnumerator GetCouponRewards_TEST2022_Success()
		{
			yield return GetCouponRewards(couponCode: "TEST2022", isSuccessExpected: true);
		}

		[UnityTest]
		public IEnumerator GetCouponRewards_IncorrectCode_Failure()
		{
			yield return GetCouponRewards(couponCode: "SUMMER1967", isSuccessExpected: false);
		}

		[UnityTest]
		public IEnumerator GetCouponRewards_TEST2022_InvalidToken_Success()
		{
			yield return TestSignInHelper.Instance.SetOldToken();
			yield return GetCouponRewards(couponCode: "TEST2022", isSuccessExpected: true);
		}

		private IEnumerator GetCouponRewards([CallerMemberName]string testName = null, string couponCode = null, bool isSuccessExpected = true)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			Action<CouponReward> onSuccess = bonus =>
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

			XsollaCatalog.Instance.GetCouponRewards(
				projectId: XsollaSettings.StoreProjectId,
				couponCode: couponCode,
				onSuccess: onSuccess,
				onError: onError);

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value == isSuccessExpected)
				TestHelper.Pass(errorMessage, testName);
			else
				TestHelper.Fail(errorMessage, testName);
		}
	}
}
