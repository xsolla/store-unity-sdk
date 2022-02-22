using System;

namespace Xsolla.UserAccount
{
	[Serializable]
	public class UserDeviceInfo
	{
		public int id;
		public string type;
		public string device;
		public string last_used_at;
	}
}
