using System;

namespace Xsolla.Core
{
	public partial class XsollaWebCallbacks
	{
		public static event Action PaymentStatusUpdate;

		public static event Action PaymentCancel;

		public void PublishPaymentStatusUpdate()
		{
			if (PaymentStatusUpdate != null)
				PaymentStatusUpdate.Invoke();
		}

		public void PublishPaymentCancel()
		{
			if (PaymentCancel != null)
				PaymentCancel.Invoke();
		}
	}
}
