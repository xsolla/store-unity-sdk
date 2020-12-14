using System;

namespace Xsolla.Login
{
	/// <summary>
	/// Found user entity.
	/// </summary>
	/// <see cref="https://go-xsolla-login.doc.srv.loc/login-api/users/get-users-search-by-nickname"/>
	[Serializable]
	public class FoundUser
	{
		/// <summary>
		/// User's avatar URL.
		/// </summary>
		public string avatar;
		/// <summary>
		/// Is the current user or not. 
		/// </summary>
		public bool is_me;
		/// <summary>
		/// Date of the last user login in the RFC3339 format.
		/// </summary>
		public string last_login;
		/// <summary>
		/// User's nickname.
		/// </summary>
		public string nickname;
		/// <summary>
		/// User's tag.
		/// </summary>
		public string tag;
		/// <summary>
		/// Date of user registration in the RFC3339 format.
		/// </summary>
		public string registered;
		/// <summary>
		/// The Xsolla Login user ID.
		/// </summary>
		public string user_id;
	}
}