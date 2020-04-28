using System;
using JetBrains.Annotations;
using Xsolla.Core;
using Xsolla.Login;

namespace Xsolla.PayStation
{
	public class XsollaPayStation : MonoSingleton<XsollaPayStation>
	{
		public void RequestToken([NotNull] Action<TokenEntity> onSuccess, [CanBeNull] Action<Error> onError)
		{
			WebRequestHelper.Instance.PostRequest(XsollaSettings.PayStationTokenRequestUrl, null, onSuccess, onError);
		}
	}
}