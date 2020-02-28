using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
			return new UserAttribute {key = string.Copy(key), permission = string.Copy(permission), value = string.Copy(value)};
		}
	}
}


