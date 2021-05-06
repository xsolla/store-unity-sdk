using Xsolla.Core;

namespace Xsolla.Demo
{
	public class DeviceIdAuth : StoreStringActionResult, ILoginAuthorization
	{
		public void TryAuth(params object[] args)
		{
#if !(UNITY_ANDROID || UNITY_IOS)
			var platform = Application.platform.ToString();
			var message = $"Device ID auth is not supported for {platform} platform";
			base.OnError?.Invoke(new Error(ErrorType.MethodIsNotAllowed, errorMessage: message));
			return;
#endif

#if UNITY_IOS
			var deviceType = Core.DeviceType.iOS;
#else
			var deviceType = Core.DeviceType.Android;
#endif
			var possibleException = DeviceIdUtil.GetDeviceInfo(deviceType, out string deviceId, out string deviceName, out string deviceModel);
			if (possibleException != null)
			{
				FailHandler(new Error(errorMessage: possibleException.Message));
				return;
			}

			var deviceInfo = $"{deviceName}:{deviceModel}";
			Debug.Log($"Trying device_type:'{deviceType}', device_id:'{deviceId}', device:'{deviceInfo}'");

			DemoController.Instance.LoginDemo.AuthViaDeviceID(deviceType, deviceInfo, deviceId,
				onSuccess: SuccessHandler,
				onError: FailHandler);
		}

		private void SuccessHandler(string token)
		{
			Debug.Log($"DeviceID Auth: Token obtained '{token}'");
			base.OnSuccess?.Invoke(token);
		}

		private void FailHandler(Error error)
		{
			Debug.LogError($"DeviceID Auth: Failed with error '{error.ToString()}'");
			base.OnError?.Invoke(error);
		}
	}
}
