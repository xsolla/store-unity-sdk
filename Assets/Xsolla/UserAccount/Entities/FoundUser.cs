using System;

namespace Xsolla.UserAccount
{
	/// <summary>
	/// Found user entity.
	/// </summary>
	/// <see href="https://go-xsolla-login.doc.srv.loc/login-api/users/get-users-search-by-nickname"/>
	[Serializable]
	public class FoundUser
	{
		/// <summary>
		/// The Xsolla Login user ID. You can find it in Publisher Account > your Login project > Users > Username/ID.
		/// </summary>
		public string user_id;
		/// <summary>
		/// User nickname.
		/// </summary>
		public string nickname;
		/// <summary>
		/// Shows whether the user who initiated a search or not.
		/// </summary>
		public bool is_me;
		/// <summary>
		/// Date of user registration in the RFC3339 format.
		/// </summary>
		public string registered;
		/// <summary>
		/// Date of the last user login in the RFC3339 format.
		/// </summary>
		public string last_login;
		/// <summary>
		/// URL of the user avatar.
		/// </summary>
		public string avatar;
		/// <summary>
		/// User tag without "#" at the beginning. Can have no unique value.
		/// </summary>
		public string tag;
	}
}
