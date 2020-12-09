namespace Xsolla.Login
{
	/// <summary>
	/// Condition for sorting users
	/// </summary>
	/// <see cref="https://developers.xsolla.com/user-account-api/user-friends/get-friends"/>
	public enum FriendsSearchResultsSortOrder
	{
		Asc,
		Desc
	}

	public static class FriendsSearchResultsSortOrderConverter
	{
		public static string GetParameter(this FriendsSearchResultsSortOrder provider)
		{
			switch (provider)
			{
				case FriendsSearchResultsSortOrder.Asc: return "asc";
				case FriendsSearchResultsSortOrder.Desc: return "desc";
				default: return string.Empty;
			}
		}
	}
}
