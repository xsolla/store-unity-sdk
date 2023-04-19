using System;
using System.Collections.Generic;

namespace Xsolla.UserAccount
{
	/// <summary>
	/// User's social friends entity.
	/// </summary>
	[Serializable]
	public class UserSocialFriends
	{
		/// <summary>
		/// Maximum number of friends that are returned at a time.
		/// </summary>
		public uint limit;
		/// <summary>
		/// Number of the elements from which the list is generated.
		/// </summary>
		public uint offset;
		/// <summary>
		/// Total number of friends that you can get.
		/// </summary>
		public uint total_count;
		/// <summary>
		/// List of data from social friends accounts.
		/// </summary>
		public List<UserSocialFriend> data;
		/// <summary>
		/// Name of a social provider.
		/// </summary>
		public string platform;
		/// <summary>
		/// Shows whether the social friends are from your game.
		/// </summary>
		public bool with_xl_uid;
	}
}
