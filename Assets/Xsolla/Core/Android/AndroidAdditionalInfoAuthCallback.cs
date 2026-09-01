#if UNITY_ANDROID
using System;
using UnityEngine;

namespace Xsolla.Core
{
	internal class AndroidAdditionalInfoAuthCallback : AndroidJavaProxy
	{
		private readonly AndroidHelper AndroidHelper;
		private readonly Action<string, string> OnSuccess;
		private readonly Action<Error> OnError;
		private readonly Action OnCancel;

		public AndroidAdditionalInfoAuthCallback(AndroidHelper androidHelper, Action<string, string> onSuccess, Action<Error> onError, Action onCancel)
			: base($"{Application.identifier}.androidProxies.AdditionalInfoAuthCallback")
		{
			AndroidHelper = androidHelper;
			OnSuccess = onSuccess;
			OnError = onError;
			OnCancel = onCancel;
		}

		public void onSuccess(string code, string token)
		{
			AndroidHelper.MainThreadExecutor.Enqueue(() => OnSuccess?.Invoke(code, token));
		}

		public void onCancel()
		{
			AndroidHelper.MainThreadExecutor.Enqueue(() => OnCancel?.Invoke());
		}

		public void onError(string message)
		{
			var safeMessage = string.IsNullOrEmpty(message)
				? "Unknown additional info auth error"
				: message;
 			AndroidHelper.MainThreadExecutor.Enqueue(() => OnError?.Invoke(new Error(ErrorType.UnknownError, errorMessage: safeMessage)));
		}
	}
}
#endif
