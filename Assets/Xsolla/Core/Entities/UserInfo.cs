using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Xsolla.Core
{
	[Serializable]
	public class UserInfo
	{
		public string id;
		public string username;
		public string nickname;
		public string name;
		public string picture;
		public string birthday;
		public string first_name;
		public string last_name;
		public string gender;
		public string email;
		public string phone;
		public List<UserGroup> groups;
		public string registered;
		public string external_id;
		public string last_login;
		public UserBan ban;
		public string country;
		public string tag;
		[CanBeNull] public string connection_information;
		[CanBeNull] public bool? is_anonymous;
		[CanBeNull] public string phone_auth;
		/// <summary>
		/// User status. Can be 'online' or 'offline'.
		/// </summary>
		[CanBeNull] public string presence;

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
			public string name;
			public bool is_default;
			public bool is_deletable;
		}
	}
}
