/// <summary>
/// Condition for sorting users.
/// </summary>
/// <see cref="https://developers.xsolla.com/user-account-api/user-friends/getusersmerelationships"/>
public enum FriendsSearchType
{
    Added,
    Pending,
    Requested,
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
            case FriendsSearchType.Pending: return "friend_requested_by";
            case FriendsSearchType.Requested: return "friend_requested";
            case FriendsSearchType.Blocked: return "blocked";
            case FriendsSearchType.BlocksMe: return "blocked_by";
            default: return string.Empty;
        }
    }
}
