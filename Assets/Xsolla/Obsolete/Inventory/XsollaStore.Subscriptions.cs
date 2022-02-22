using System;
using JetBrains.Annotations;
using Xsolla.Core;
using Xsolla.Subscriptions;

namespace Xsolla.Store
{
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		[Obsolete("Use XsollaSubscriptions instead")]
		public void GetSubscriptions(string projectId, [NotNull] Action<SubscriptionItems> onSuccess, [CanBeNull] Action<Error> onError)
			=> XsollaSubscriptions.Instance.GetSubscriptions(projectId, onSuccess, onError);
	}
}
