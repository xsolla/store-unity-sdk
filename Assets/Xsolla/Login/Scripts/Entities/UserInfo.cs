using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Xsolla.Login
{
	[Serializable]
	public class UserInfo
	{
		[Serializable]
		public class UserBan
		{
			public string date_from;
			public string date_to;
			public string reason;
		}
		[Serializable]
		public class UserGroup
		{
			public int id;
			public bool is_default;
			public bool is_deletable;
			public string name;
		}
		public UserBan ban;
		public string birthday;
		public string connection_information;
		public string country;
		public string email;
		public string external_id;
		public string first_name;
		public string gender;
		public List<UserGroup> groups;
		public string id;
		public string last_login;
		public string last_name;
		public string name;
		public string nickname;
		public string tag;
		public string phone;
		public string picture;
		public string registered;
		public string username;
		/// <summary>
		/// User status. Can be 'online' or 'offline'.
		/// </summary>
		[CanBeNull] public string presence;
	}
}