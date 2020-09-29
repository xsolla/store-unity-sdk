using System;
using System.Collections.Generic;

namespace Xsolla.Login
{
	/// <summary>
	/// User's friends entity.
	/// </summary>
	/// <see cref="https://developers.xsolla.com/user-account-api/user-friends/get-friends"/>
	[Serializable]
	public class UserFriendsEntity
	{
		/// <summary>
		/// TEMPORARY UNDER MAINTENANCE.
		/// </summary>
		public string next_after;
		/// <summary>
		/// TEMPORARY UNDER MAINTENANCE.
		/// </summary>
		public string next_url;
		/// <summary>
		/// Friends details.
		/// </summary>
		public List<UserFriendEntity> relationships;
	}
}
