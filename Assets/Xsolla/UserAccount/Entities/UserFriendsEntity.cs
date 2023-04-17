using System;
using System.Collections.Generic;

namespace Xsolla.UserAccount
{
	/// <summary>
	/// User's friends entity.
	/// </summary>
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
