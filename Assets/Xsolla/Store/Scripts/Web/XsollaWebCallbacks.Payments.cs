using System;
using Newtonsoft.Json;
using Xsolla.Store;

namespace Xsolla.Core
{
	public partial class XsollaWebCallbacks
	{
		public static event Action<PaymentInfo> PaymentStatusUpdate;

		public static event Action PaymentCancel;

		public void PublishPaymentStatusUpdate(string json)
		{
			var info = JsonConvert.DeserializeObject<PaymentInfo>(json);
			PaymentStatusUpdate?.Invoke(info);
		}

		public void PublishPaymentCancel()
		{
			PaymentCancel?.Invoke();
		}
	}
}