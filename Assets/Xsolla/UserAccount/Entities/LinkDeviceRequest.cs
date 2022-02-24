using System;

namespace Xsolla.UserAccount
{
	[Serializable]
	public class LinkDeviceRequest
	{
		public string device;
		public string device_id;

		public LinkDeviceRequest(string device, string device_id)
		{
			this.device = device;
			this.device_id = device_id;
		}
	}
}
