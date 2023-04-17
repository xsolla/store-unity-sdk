using System;
using Xsolla.Core;

namespace Xsolla.UserAccount
{
	/// <summary>
	/// User's friend entity.
	/// </summary>
	/// <see href="https://developers.xsolla.com/user-account-api/user-friends/get-friends"/>
	[Serializable]
	public class UserFriendEntity
	{
		/// <summary>
		/// Type of the requested user. Can be:
		///none if there is no action initiated for the user
		///friend if the user is in the friend list of the one with the specified JWT
		///friend_requested if the friend request is sent to the user by the one with the specified JWT
		///blocked if the user is blocked by the one with the specified JWT
		/// </summary>
		public string status_outgoing;
		/// <summary>
		/// Type of the user. Can be:
		///none if there is no action initiated for the user
		///friend if the user is in the friend list of the requested one
		///friend_requested if the friend request is sent to the user by the requested one
		///blocked if the user is blocked by the requested one
		/// </summary>
		public string status_incoming;
		/// <summary>
		/// User details.
		/// </summary>
		public UserInfo user;
		/// <summary>
		/// Time passed since the latest action of adding a friend to the friend list or banning them in seconds.
		/// </summary>
		public float updated;

		public bool IsOnline()
		{
			return !string.IsNullOrEmpty(user.presence) && user.presence.Equals("online");
		}
	}
}
