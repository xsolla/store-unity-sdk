using System;

namespace Xsolla.Core
{
	public partial class XsollaWebCallbacks : MonoSingleton<XsollaWebCallbacks>
	{
		public static event Action<object> MessageReceived;

		public void PublishMessage(object message)
		{
			MessageReceived?.Invoke(message);
		}

		public override void Init()
		{
			base.Init();
			gameObject.name = "XsollaWebCallbacks";
		}
	}
}