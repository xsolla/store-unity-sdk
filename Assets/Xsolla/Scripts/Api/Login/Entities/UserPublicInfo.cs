using System;

namespace Xsolla.Login
{
	/// <summary>
	/// Public user info entity.
	/// </summary>
	/// <see cref="https://go-xsolla-login.doc.srv.loc/login-api/users/get-public-user-profile"/>
	[Serializable]
	public class UserPublicInfo
	{
		/// <summary>
		/// User's avatar URL.
		/// </summary>
		public string avatar;
		/// <summary>
		/// Date of the last user login in the RFC3339 format.
		/// </summary>
		public string last_login;
		/// <summary>
		/// User's nickname.
		/// </summary>
		public string nickname;
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