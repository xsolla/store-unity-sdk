using System;
using JetBrains.Annotations;
using Xsolla.Core;

namespace Xsolla.PayStation
{
	public class XsollaPayStation : MonoSingleton<XsollaPayStation>
	{
		public void RequestToken([NotNull] Action<Token> onSuccess, [CanBeNull] Action<Error> onError)
		{
			WebRequestHelper.Instance.PostRequest(XsollaSettings.PayStationTokenRequestUrl, null, onSuccess, onError);
		}
	}
}