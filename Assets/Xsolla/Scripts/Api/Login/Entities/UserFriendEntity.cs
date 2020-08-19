using System;

namespace Xsolla.Login
{
	/// <summary>
	/// User's friend entity.
	/// </summary>
	/// <see cref="https://developers.xsolla.com/user-account-api/user-friends/get-friends"/>
	[Serializable]
	public class UserFriendEntity
	{
		/// <summary>
		/// Type of the user with the specified JWT. 
		/// </summary>
		public string status_incoming;
		/// <summary>
		/// Type of the requested user. 
		/// </summary>
		public string status_outgoing;
		/// <summary>
		/// User's nickname. Format: [seconds.milliseconds].
		/// </summary>
		public float updated;
		/// <summary>
		/// User details.
		/// </summary>
		public UserInfo user;

		public bool IsOnline()
		{
			return !string.IsNullOrEmpty(user.presence) && user.presence.Equals("online");
		}
	}
}