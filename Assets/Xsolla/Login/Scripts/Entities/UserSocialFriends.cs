using System;
using System.Collections.Generic;

namespace Xsolla.Login
{
	/// <summary>
	/// User's social friends entity.
	/// </summary>
	/// <see cref="https://developers.xsolla.com/login-api/methods/users/get-users-friends"/>
	[Serializable]
	public class UserSocialFriends
	{
		/// <summary>
		/// List of data from social friends accounts.
		/// </summary>
		public List<UserSocialFriend> data;
		/// <summary>
		/// Maximum number of friends that are returned at a time.
		/// </summary>
		public uint limit;
		/// <summary>
		/// Number of elements from which the list is generated.
		/// </summary>
		public uint offset;
		/// <summary>
		/// Total number of friends that you can get.
		/// </summary>
		public uint total_count;
		/// <summary>
		/// Name of social provider.
		/// </summary>
		public string platform;
	}
}
