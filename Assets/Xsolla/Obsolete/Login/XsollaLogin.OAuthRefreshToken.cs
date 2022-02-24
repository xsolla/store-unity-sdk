using System;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Login
{
	public partial class XsollaLogin : MonoSingleton<XsollaLogin>
	{
		[Obsolete("Use XsollaAuth instead")]
		public bool IsOAuthTokenRefreshInProgress
			=> XsollaAuth.Instance.IsOAuthTokenRefreshInProgress;

		[Obsolete("Use XsollaAuth instead")]
		public void ExchangeCodeToToken(string code, Action<string> onSuccessExchange = null, Action<Error> onError = null)
			=> XsollaAuth.Instance.ExchangeCodeToToken(code, onSuccessExchange, onError);
	}
}
