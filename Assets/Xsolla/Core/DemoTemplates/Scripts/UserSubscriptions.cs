using System;
using System.Collections.Generic;
using System.Linq;
using Xsolla.Core;

namespace Xsolla.Store
{
	public class UserSubscriptions : MonoSingleton<UserSubscriptions>
	{
		private List<SubscriptionItem> _items = new List<SubscriptionItem>();

		public List<SubscriptionItem> GetItems() => _items;

		public bool IsEmpty()
		{
			return !GetItems().Any();
		}

		public bool IsSubscription(string sku)
		{
			return GetItems().Exists(i => i.sku == sku);
		}

		public void UpdateSupscriptions(Action<List<SubscriptionItem>> onSuccess = null, Action<Error> onError = null)
		{
			XsollaStore.Instance.GetSubscriptions(XsollaSettings.StoreProjectId, items =>
				{
					_items = items.items.ToList();
					onSuccess?.Invoke(GetItems());
				},
				onError);
		}
	}
}