using System;

namespace Xsolla.Auth
{
	[Serializable]
	public class LoginDeviceIdRequest
	{
		public string device;
		public string device_id;

		public LoginDeviceIdRequest(string device, string device_id)
		{
			this.device = device;
			this.device_id = device_id;
		}
	}
}
