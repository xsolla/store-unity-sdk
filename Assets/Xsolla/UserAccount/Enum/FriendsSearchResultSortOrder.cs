namespace Xsolla.UserAccount
{
	/// <summary>
	/// Condition for sorting users
	/// </summary>
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
