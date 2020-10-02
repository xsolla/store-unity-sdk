/// <summary>
/// Condition for sorting users.
/// </summary>
/// <see cref="https://developers.xsolla.com/user-account-api/user-friends/get-friends"/>
public enum FriendsSearchResultsSort
{
    ByNickname,
    ByUpdated
}

public static class FriendsSearchResultsSortConverter
{
    public static string GetParameter(this FriendsSearchResultsSort provider)
    {
        switch (provider)
        {
            case FriendsSearchResultsSort.ByNickname: return "by_nickname";
            case FriendsSearchResultsSort.ByUpdated: return "by_updated";
            default: return string.Empty;
        }
    }
}
