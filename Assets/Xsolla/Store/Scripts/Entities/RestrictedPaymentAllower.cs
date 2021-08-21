using System;

namespace Xsolla.Demo
{
	public class RestrictedPaymentAllower
    {
		public Action<int> OnRestrictedPayment;
		public Action<bool> OnAllowed;
    }
}
