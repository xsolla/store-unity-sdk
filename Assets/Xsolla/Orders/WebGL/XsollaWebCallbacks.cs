using System;
using Xsolla.Core;

namespace Xsolla.Orders
{
	public class XsollaWebCallbacks : MonoSingleton<XsollaWebCallbacks>
	{
		public event Action OnPaymentStatusUpdate;
		public event Action OnPaymentCancel;

		public override void Init()
		{
			base.Init();
			gameObject.name = "XsollaWebCallbacks";
		}

		public void PublishPaymentStatusUpdate()
		{
			OnPaymentStatusUpdate?.Invoke();
		}

		public void PublishPaymentCancel()
		{
			OnPaymentCancel?.Invoke();
		}
	}
}