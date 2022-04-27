using System;
using System.Collections.Generic;
using Xsolla.Auth;
using Xsolla.Core;
using Xsolla.UserAccount;

namespace Xsolla.Login
{
	public partial class XsollaLogin : MonoSingleton<XsollaLogin>
	{
		[Obsolete("Use XsollaAuth instead")]
		public void AuthViaDeviceID(DeviceType deviceType, string device, string deviceId, string payload = null, string state = null, Action<string> onSuccess = null, Action<Error> onError = null)
			=> XsollaAuth.Instance.AuthViaDeviceID(deviceType, device, deviceId, payload, state, onSuccess, onError);

		[Obsolete("Use XsollaUserAccount instead")]
		public void AddUsernameEmailAuthToAccount(string username, string password, string email, int? promoEmailAgreement = null, Action<bool> onSuccess = null, Action<Error> onError = null)
			=> XsollaUserAccount.Instance.AddUsernameEmailAuthToAccount(username, password, email, promoEmailAgreement, onSuccess, onError);

		[Obsolete("Use XsollaUserAccount instead")]
		public void GetUserDevices(Action<List<UserDeviceInfo>> onSuccess = null, Action<Error> onError = null)
			=> XsollaUserAccount.Instance.GetUserDevices(onSuccess, onError);

		[Obsolete("Use XsollaUserAccount instead")]
		public void LinkDeviceToAccount(DeviceType deviceType, string device, string deviceId, Action onSuccess = null, Action<Error> onError = null)
			=> XsollaUserAccount.Instance.LinkDeviceToAccount(deviceType, device, deviceId, onSuccess, onError);

		[Obsolete("Use XsollaUserAccount instead")]
		public void UnlinkDeviceFromAccount(int id, Action onSuccess = null, Action<Error> onError = null)
			=> XsollaUserAccount.Instance.UnlinkDeviceFromAccount(id, onSuccess, onError);
	}
}
