using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.Tests
{
	public class XsollaCatalogTests
	{
		[OneTimeSetUp]
		[OneTimeTearDown]
		public void Clear() => TestHelper.Clear();

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

		[UnityTest]
		public IEnumerator GetCatalog_Personalized_HasPersonalizedItem()
		{
			yield return GetCatalog(personalized: true);
		}

		[UnityTest]
		public IEnumerator GetCatalog_NonPersonalized_NoPersonalizedItem()
		{
			yield return GetCatalog(personalized: false);
		}

		[UnityTest]
		public IEnumerator GetCatalog_PromotionItem_HasPromotionItem()
		{
			yield return GetCatalog(personalized: true, checkPromotion: true);
		}

		private IEnumerator GetCatalog([CallerMemberName]string testName = null, bool defaultValues = true, bool personalized = false, bool checkPromotion = false)
		{
			if (personalized)
				yield return TestSignInHelper.Instance.CheckSession();
			else
				Token.Instance = null;

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

				var hasPersonalizedItem = items.items.FirstOrDefault(x => x.sku.Equals("xsollus_personalized_item")) != null;
				if (personalized != hasPersonalizedItem)
				{
					errorMessage = $"PERSONALIZATION ERROR, EXPECTED {personalized} ACTUAL {hasPersonalizedItem}";
					success = false;
					return;
				};

				if (checkPromotion && !IsPromotionItemPreset(items.items, out errorMessage))
				{
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

		[UnityTest]
		public IEnumerator GetCatalogSimplified_Personalized_HasPersonalizedItem()
		{
			yield return GetCatalogSmplified(personalized: true);
		}

		[UnityTest]
		public IEnumerator GetCatalogSimplified_NonPersonalized_NoPersonalizedItem()
		{
			yield return GetCatalogSmplified(personalized: false);
		}

		// [UnityTest]
		// public IEnumerator GetCatalogSimplified_PromotionItem_HasPromotionItem()
		// {
		// 	yield return GetCatalogSmplified(checkPromotion: true);
		// }

		private IEnumerator GetCatalogSmplified([CallerMemberName]string testName = null, bool defaultValues = true, bool personalized = false, bool checkPromotion = false)
		{
			if (personalized)
				yield return TestSignInHelper.Instance.CheckSession();
			else
				Token.Instance = null;

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

				var hasPersonalizedItem = items.items.FirstOrDefault(x => x.sku.Equals("xsollus_personalized_item")) != null;
				if (personalized != hasPersonalizedItem)
				{
					errorMessage = $"PERSONALIZATION ERROR, EXPECTED {personalized} ACTUAL {hasPersonalizedItem}";
					success = false;
					return;
				};

				//We can not follow DRY here because items are different
				if (checkPromotion)
				{
					var itemWithPromotion = items.items.FirstOrDefault(x => x.sku.Equals("sdk_team_special"));
					if (itemWithPromotion == null)
					{
						errorMessage = "PROMOTION ERROR, EXPECTED ITEM NOT FOUND";
						success = false;
						return;
					}

					var promotionContainer = itemWithPromotion.promotions;
					if (promotionContainer == null || promotionContainer.Length == 0)
					{
						errorMessage = "PROMOTION ERROR, PROMOTION CONTAINER IS NULL OR EMPTY";
						success = false;
						return;
					}

					var promotionItem = promotionContainer[0];
					if (string.IsNullOrEmpty(promotionItem.name) ||
						string.IsNullOrEmpty(promotionItem.date_start) ||
						string.IsNullOrEmpty(promotionItem.date_end))
					{
						errorMessage = $"PROMOTION ERROR, PROMOTION FIELDS: name:'{promotionItem.name}' date_start:'{promotionItem.date_start}' date_end:'{promotionItem.date_end}'";
						success = false;
						return;
					}

					UnityEngine.Debug.Log("PERSONALIZATION PASSED SUCCESSFULLY");
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

		[UnityTest]
		public IEnumerator GetGroupItems_PromotionItem_HasPromotionItem()
		{
			yield return GetGroupItems(checkPromotion: true);
		}

		private IEnumerator GetGroupItems([CallerMemberName]string testName = null, bool defaultValues = true, bool checkPromotion = false)
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

				if (checkPromotion && !IsPromotionItemPreset(items.items, out errorMessage))
				{
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

		[UnityTest]
		public IEnumerator GetVirtualCurrencyPackagesList_Personalized_HasPersonalizedItem()
		{
			yield return GetVirtualCurrencyPackagesList(personalized: true);
		}

		[UnityTest]
		public IEnumerator GetVirtualCurrencyPackagesList_NonPersonalized_NoPersonalizedItem()
		{
			yield return GetVirtualCurrencyPackagesList(personalized: false);
		}

		private IEnumerator GetVirtualCurrencyPackagesList([CallerMemberName]string testName = null, bool defaultValues = true, bool personalized = false)
		{
			if (personalized)
				yield return TestSignInHelper.Instance.CheckSession();
			else
				Token.Instance = null;

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

				var hasPersonalizedItem = items.items.FirstOrDefault(x => x.sku.Equals("crystal_pack_specialxsollus")) != null;
				if (personalized != hasPersonalizedItem)
				{
					errorMessage = $"PERSONALIZATION ERROR, EXPECTED {personalized} ACTUAL {hasPersonalizedItem}";
					success = false;
					return;
				};

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

		[UnityTest]
		public IEnumerator GetBundles_Personalized_HasPersonalizedItem()
		{
			yield return GetBundles(personalized: true);
		}

		[UnityTest]
		public IEnumerator GetBundles_NonPersonalized_NoPersonalizedItem()
		{
			yield return GetBundles(personalized: false);
		}

		private IEnumerator GetBundles([CallerMemberName]string testName = null, bool defaultValues = true, bool personalized = false)
		{
			if (personalized)
				yield return TestSignInHelper.Instance.CheckSession();
			else
				Token.Instance = null;

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

				var hasPersonalizedItem = items.items.FirstOrDefault(x => x.sku.Equals("sdk_special_pack_forxsollus")) != null;
				if (personalized != hasPersonalizedItem)
				{
					errorMessage = $"PERSONALIZATION ERROR, EXPECTED {personalized} ACTUAL {hasPersonalizedItem}";
					success = false;
					return;
				};

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

		private bool IsPromotionItemPreset(StoreItem[] items, out string errorMessage)
		{
			var itemWithPromotion = items.FirstOrDefault(x => x.sku.Equals("sdk_team_special"));
			if (itemWithPromotion == null)
			{
				errorMessage = "PROMOTION ERROR, EXPECTED ITEM NOT FOUND";
				return false;
			}

			var promotionContainer = itemWithPromotion.promotions;
			if (promotionContainer == null || promotionContainer.Length == 0)
			{
				errorMessage = "PROMOTION ERROR, PROMOTION CONTAINER IS NULL OR EMPTY";
				return false;
			}

			var promotionItem = promotionContainer[0];
			if (string.IsNullOrEmpty(promotionItem.name) ||
				string.IsNullOrEmpty(promotionItem.date_start) ||
				string.IsNullOrEmpty(promotionItem.date_end))
			{
				errorMessage = $"PROMOTION ERROR, PROMOTION FIELDS: name:'{promotionItem.name}' date_start:'{promotionItem.date_start}' date_end:'{promotionItem.date_end}'";
				return false;
			}

			UnityEngine.Debug.Log("PERSONALIZATION PASSED SUCCESSFULLY");
			errorMessage = null;
			return true;
		}
	}
}
