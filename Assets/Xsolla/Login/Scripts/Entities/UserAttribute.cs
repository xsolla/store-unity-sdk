using System;

namespace Xsolla.Login
{
	[Serializable]
	public class UserAttribute
	{
		public UserAttribute()
		{
			key = string.Empty;
		}
		
		public string key;
		public string permission;
		public string value;

		public UserAttribute GetCopy()
		{
			return new UserAttribute {key = this.key, permission = this.permission, value = this.value};
		}
	}
}
