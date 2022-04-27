using System;
using JetBrains.Annotations;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.Store
{
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		[Obsolete("Use XsollaCatalog instead")]
		public void GetCatalogShort(string projectId, Action<StoreItemShortCollection> onSuccess, Action<Error> onError = null, string locale = null)
			=> XsollaCatalog.Instance.GetCatalogSimplified(projectId, onSuccess, onError, locale);

		[Obsolete("Use XsollaCatalog instead")]
		public void GetCatalog(string projectId, [NotNull] Action<StoreItems> onSuccess, [CanBeNull] Action<Error> onError, int limit = 50, int offset = 0, [CanBeNull] string locale = null, string additionalFields = "long_description", [CanBeNull] string country = null)
			=> XsollaCatalog.Instance.GetCatalog(projectId, onSuccess, onError, limit, offset, locale, additionalFields, country);

		[Obsolete("Use XsollaCatalog instead")]
		public void GetBundle(string projectId, string sku, [NotNull] Action<BundleItem> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, [CanBeNull] string country = null)
			=> XsollaCatalog.Instance.GetBundle(projectId, sku, onSuccess, onError, locale, country);

		[Obsolete("Use XsollaCatalog instead")]
		public void GetBundles(string projectId, [NotNull] Action<BundleItems> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, int limit = 50, int offset = 0, string additionalFields = null, [CanBeNull] string country = null)
			=> XsollaCatalog.Instance.GetBundles(projectId, onSuccess, onError, limit, offset, locale, additionalFields, country);

		[Obsolete("Use XsollaCatalog instead")]
		public void GetGroupItems(string projectId, string groupExternalId, [NotNull] Action<StoreItems> onSuccess, [CanBeNull] Action<Error> onError, int? limit = null, int? offset = null, [CanBeNull] string locale = null, string additionalFields = null, string country = null)
			=> XsollaCatalog.Instance.GetGroupItems(projectId, groupExternalId, onSuccess, onError, limit, offset, locale, additionalFields, country);

		[Obsolete("Use XsollaCatalog instead")]
		public void GetItemGroups(string projectId, [NotNull] Action<Groups> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, int offset = 0, int limit = 50)
			=> XsollaCatalog.Instance.GetItemGroups(projectId, onSuccess, onError, limit, offset, locale);
	}
}
