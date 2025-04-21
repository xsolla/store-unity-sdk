using System;
using Xsolla.Core;

namespace Xsolla.ReadyToUseStore
{
	internal interface IAuthenticator
	{
		void Execute(Action onSuccess, Action<Error> onError, Action onCancel, Action onSkip);
	}
}