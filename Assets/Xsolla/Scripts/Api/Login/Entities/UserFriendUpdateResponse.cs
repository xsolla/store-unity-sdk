using System;

namespace Xsolla.Login
{
	/// <summary>
	/// User's friend update response entity.
	/// </summary>
	/// <see cref="https://developers.xsolla.com/user-account-api/user-friends/postusersmerelationships"/>
	[Serializable]
	public class UserFriendUpdateResponse
	{
		/// <summary>
		/// Type of the user with the specified JWT.
		/// </summary>
		public string status_incoming;
		/// <summary>
		/// Type of the requested user.
		/// </summary>
		public string status_outgoing;
	}
}