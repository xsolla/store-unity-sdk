using System;
using JetBrains.Annotations;

namespace Xsolla.UserAccount
{
	/// <summary>
	/// User's social friend entity.
	/// </summary>
	/// <see cref="https://developers.xsolla.com/login-api/methods/users/get-users-friends"/>
	[Serializable]
	public class UserSocialFriend
	{
		/// <summary>
		/// Friend’s name from a social provider.
		/// </summary>
		public string name;
		/// <summary>
		/// Name of a social provider.
		/// </summary>
		public string platform;
		/// <summary>
		/// User ID from a social provider.
		/// </summary>
		public string user_id;
		/// <summary>
		/// Friend's avatar from a social provider.
		/// </summary>
		[CanBeNull] public string avatar;
		/// <summary>
		/// User tag without "#" at the beginning. Can have no unique value and can be used in the Search users by nickname call.
		/// </summary>
		[CanBeNull] public string tag;
		/// <summary>
		/// The Xsolla Login user ID. You can find it in Publisher Account > your Login project > Users > Username/ID.
		/// </summary>
		[CanBeNull] public string xl_uid;
	}
}
