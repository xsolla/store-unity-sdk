using Xsolla.Core;

namespace Xsolla.Auth
{
	public class DeviceInfo
	{
		// Type of the device. Can be `android` and `ios`.
		public DeviceType DeviceType;

		// For Android, it is an [ANDROID_ID](https://developer.android.com/reference/android/provider/Settings.Secure#ANDROID_ID) constant.<br/>
		// For iOS, it is an [identifierForVendor](https://developer.apple.com/documentation/uikit/uidevice/1620059-identifierforvendor?language=objc) property.
		public string DeviceId;

		// Manufacturer name of the device.
		public string DeviceModel;

		// Model name of the device.
		public string DeviceName;
	}
}