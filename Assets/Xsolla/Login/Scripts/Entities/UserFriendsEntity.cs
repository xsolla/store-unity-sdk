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
		/// Friends details.
		/// </summary>
		public List<UserFriendEntity> relationships;
		/// <summary>
		/// URL of the request for using this call for the next time.
		/// </summary>
		public string next_url;
		/// <summary>
		/// Value of the after parameter that should be passed while requesting this call for the next time.
		/// </summary>
		public string next_after;
	}
}
