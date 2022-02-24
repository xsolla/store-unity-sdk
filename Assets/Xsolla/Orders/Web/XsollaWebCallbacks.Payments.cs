using System;

namespace Xsolla.Core
{
	public partial class XsollaWebCallbacks
	{
		public static event Action PaymentStatusUpdate;

		public static event Action PaymentCancel;

		public void PublishPaymentStatusUpdate()
		{
			PaymentStatusUpdate?.Invoke();
		}

		public void PublishPaymentCancel()
		{
			PaymentCancel?.Invoke();
		}
	}
}