/// <summary>
/// Condition for sorting users.
/// </summary>
/// <see cref="https://developers.xsolla.com/user-account-api/user-friends/getusersmerelationships"/>
public enum FriendsSearchResultsSort
{
    ByName,
    ByUpdated
}

public static class FriendsSearchResultsSortConverter
{
    public static string GetParameter(this FriendsSearchResultsSort provider)
    {
        switch (provider)
        {
            case FriendsSearchResultsSort.ByName: return "by_name";
            case FriendsSearchResultsSort.ByUpdated: return "by_updated";
            default: return string.Empty;
        }
    }
}
