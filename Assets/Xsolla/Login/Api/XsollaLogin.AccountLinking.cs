using System;
using Xsolla.Core;

namespace Xsolla.Login
{
	public partial class XsollaLogin : MonoSingleton<XsollaLogin>
	{
		[Obsolete("Use XsollaAuth instead")]
		public void SignInConsoleAccount(string userId, string platform, Action<string> successCase, Action<Error> failedCase)
			=> XsollaAuth.Instance.SignInConsoleAccount(userId, platform, successCase, failedCase);

		[Obsolete("Use XsollaUserAccount instead")]
		public void RequestLinkingCode(Action<LinkingCode> onSuccess, Action<Error> onError)
			=> XsollaUserAccount.Instance.RequestLinkingCode(onSuccess, onError);

		[Obsolete("Use XsollaUserAccount instead")]
		public void LinkConsoleAccount(string userId, string platform, string confirmationCode, Action onSuccess, Action<Error> onError)
			=> XsollaUserAccount.Instance.LinkConsoleAccount(userId, platform, confirmationCode, onSuccess, onError);
	}
}
