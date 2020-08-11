using System;

namespace Xsolla.Login
{
	/// <summary>
	/// User's social friend entity.
	/// </summary>
	/// <see cref="https://developers.xsolla.com/login-api/methods/users/get-users-friends"/>
	[Serializable]
	public class UserSocialFriend
	{
		/// <summary>
		/// Friend's avatar from a social provider.
		/// </summary>
		public string avatar;
		/// <summary>
		/// Friend's name from a social provider.
		/// </summary>
		public string name;
		/// <summary>
		/// Name of a social provider.
		/// </summary>
		public string platform;
		/// <summary>
		/// User ID from a social provider
		/// </summary>
		public string user_id;
		/// <summary>
		/// The Xsolla Login user ID.
		/// </summary>
		public string xl_uid;
	}
}