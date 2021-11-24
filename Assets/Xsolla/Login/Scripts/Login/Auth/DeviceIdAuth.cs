using Xsolla.Core;

namespace Xsolla.Demo
{
	public class DeviceIdAuth : LoginAuthorization
	{
		public override void TryAuth(params object[] args)
		{
			var supported = true;
#if !(UNITY_ANDROID || UNITY_IOS) || UNITY_EDITOR
			var message = "Device ID auth is not supported for this platform";
			if (base.OnError != null)
				base.OnError.Invoke(new Error(ErrorType.MethodIsNotAllowed, errorMessage: message));
			supported = false;
#endif
			if (!supported)
				return;

#if UNITY_IOS
			var deviceType = Core.DeviceType.iOS;
#else
			var deviceType = Core.DeviceType.Android;
#endif
			string deviceId; string deviceName; string deviceModel;
			var possibleException = DeviceIdUtil.GetDeviceInfo(deviceType, out deviceId, out deviceName, out deviceModel);
			if (possibleException != null)
			{
				FailHandler(new Error(errorMessage: possibleException.Message));
				return;
			}

			var deviceInfo = string.Format("{0}:{1}", deviceName, deviceModel);
			Debug.Log(string.Format("Trying device_type:'{0}', device_id:'{1}', device:'{2}'", deviceType, deviceId, deviceInfo));

			SdkLoginLogic.Instance.AuthViaDeviceID(deviceType, deviceInfo, deviceId,
				onSuccess: SuccessHandler,
				onError: FailHandler);
		}

		private void SuccessHandler(string token)
		{
			Debug.Log(string.Format("DeviceID Auth: Token obtained '{0}'", token));
			if (base.OnSuccess != null)
				base.OnSuccess.Invoke(token);
		}

		private void FailHandler(Error error)
		{
			Debug.LogError(string.Format("DeviceID Auth: Failed with error '{0}'", error.ToString()));
			if (base.OnError != null)
				base.OnError.Invoke(error);
		}
	}
}
