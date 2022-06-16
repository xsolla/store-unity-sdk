using System;

namespace Xsolla.Subscriptions
{
	[Serializable]
	public class PlanItems
	{
		public PlanItem[] items;
		public bool has_more;
	}
}