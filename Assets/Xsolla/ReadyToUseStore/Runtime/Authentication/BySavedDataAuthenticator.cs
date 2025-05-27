using System;
using Xsolla.Core;

namespace Xsolla.ReadyToUseStore
{
	internal class BySavedDataAuthenticator : IAuthenticator
	{
		public void Execute(Action onSuccess, Action<Error> onError, Action onCancel, Action onSkip)
		{
			if (XsollaToken.TryLoadInstance())
				onSuccess?.Invoke();
			else
				onSkip?.Invoke();
		}
	}
}