using System;

namespace Xsolla.Login
{
	/// <summary>
	/// User's friend entity.
	/// </summary>
	/// <see cref="https://developers.xsolla.com/user-account-api/user-friends/getusersmerelationships"/>
	[Serializable]
	public class UserFriend
	{
		/// <summary>
		/// User status. Can be 'online' or 'offline'.
		/// </summary>
		public string presence;
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
			return presence.Equals("online");
		}
	}
}