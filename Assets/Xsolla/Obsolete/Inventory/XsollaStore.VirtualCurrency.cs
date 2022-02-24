using System;
using JetBrains.Annotations;
using Xsolla.Catalog;
using Xsolla.Core;
using Xsolla.Inventory;

namespace Xsolla.Store
{
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		[Obsolete("Use XsollaInventory instead")]
		public void GetVirtualCurrencyBalance(string projectId, [NotNull] Action<VirtualCurrencyBalances> onSuccess, [CanBeNull] Action<Error> onError)
			=> XsollaInventory.Instance.GetVirtualCurrencyBalance(projectId, onSuccess, onError);

		[Obsolete("Use XsollaCatalog instead")]
		public void GetVirtualCurrencyList(string projectId, [NotNull] Action<VirtualCurrencyItems> onSuccess, [CanBeNull] Action<Error> onError, int limit = 50, int offset = 0, [CanBeNull] string locale = null, string additionalFields = null, string country = null)
			=> XsollaCatalog.Instance.GetVirtualCurrencyList(projectId, onSuccess, onError, limit, offset, locale, additionalFields, country);

		[Obsolete("Use XsollaCatalog instead")]
		public void GetVirtualCurrencyPackagesList(string projectId, [NotNull] Action<VirtualCurrencyPackages> onSuccess, [CanBeNull] Action<Error> onError, int limit = 50, int offset = 0, [CanBeNull] string locale = null, string additionalFields = null, string country = null)
			=> XsollaCatalog.Instance.GetVirtualCurrencyPackagesList(projectId, onSuccess, onError, limit, offset, locale, additionalFields, country);
	}
}
