namespace Xsolla.UserAccount
{
	/// <summary>
	/// Condition for sorting users
	/// </summary>
	public enum FriendsSearchType
	{
		Added,
		Requested,
		Pending,
		Blocked,
		BlocksMe
	}

	public static class FriendsSearchTypeConverter
	{
		public static string GetParameter(this FriendsSearchType provider)
		{
			switch (provider)
			{
				case FriendsSearchType.Added: return "friends";
				case FriendsSearchType.Requested: return "friend_requested";
				case FriendsSearchType.Pending: return "friend_requested_by";
				case FriendsSearchType.Blocked: return "blocked";
				case FriendsSearchType.BlocksMe: return "blocked_by";
				default: return string.Empty;
			}
		}
	}
}
