using System;

namespace Xsolla.UserAccount
{
	/// <summary>
	/// User's friend entity.
	/// </summary>
	/// <see cref="https://developers.xsolla.com/user-account-api/user-friends/postusersmerelationships"/>
	[Serializable]
	public class UserFriendUpdate
	{
		/// <summary>
		/// Type of the action.
		/// </summary>
		public string action;
		/// <summary>
		/// The Xsolla Login user ID to change relationship with.
		/// </summary>
		public string user;
	}
}
