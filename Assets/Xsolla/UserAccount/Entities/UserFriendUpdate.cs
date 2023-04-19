using System;

namespace Xsolla.UserAccount
{
	/// <summary>
	/// User's friend entity.
	/// </summary>
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
