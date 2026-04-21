using System;

namespace Xsolla.Subscriptions
{
	public enum SubscriptionStatus
	{
		Unknown,
		[Obsolete]
		New,
		Active,
		Canceled,
		NonRenewing,
		[Obsolete]
		Pause
	}
}