using System;
using System.Collections.Generic;

namespace Xsolla.Login
{
	/// <summary>
	/// Found user entity.
	/// </summary>
	/// <see cref="https://go-xsolla-login.doc.srv.loc/login-api/users/get-users-search-by-nickname"/>
	[Serializable]
	public class FoundUsers
	{
		/// <summary>
		/// List of data from social friends accounts.
		/// </summary>
		public List<FoundUser> users;
		/// <summary>
		/// Number of the elements from which the list is generated.
		/// </summary>
		public uint offset;
		/// <summary>
		/// Total number of users that you can get.
		/// </summary>
		public uint total_count;
	}
}
