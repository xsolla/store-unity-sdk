/// <summary>
/// Type of the action.
/// </summary>
/// <see cref="https://developers.xsolla.com/user-account-api/user-friends/postusersmerelationships"/>
public enum FriendAction
{
    /// <summary>
    /// Send friend request
    /// </summary>
    SendInviteRequest,
    /// <summary>
    /// Cancel sent friend request
    /// </summary>
    CancelRequest,
    /// <summary>
    /// Confirm  friend request
    /// </summary>
    AcceptInvite,
    /// <summary>
    /// Cancel received friend request
    /// </summary>
    DenyInvite,
    /// <summary>
    /// Delete user from the friend list
    /// </summary>
    RemoveFriend,
    /// <summary>
    /// Block user
    /// </summary>
    BlockFriend,
    /// <summary>
    /// Unblock user
    /// </summary>
    UnblockFriend
}

public static class FriendActionConverter
{
    public static string GetParameter(this FriendAction provider)
    {
        switch (provider)
        {
            case FriendAction.SendInviteRequest: return "friend_request_add";
            case FriendAction.CancelRequest: return "friend_request_cancel";
            case FriendAction.AcceptInvite: return "friend_request_approve";
            case FriendAction.DenyInvite: return "friend_request_deny";
            case FriendAction.RemoveFriend: return "friend_remove";
            case FriendAction.BlockFriend: return "block";
            case FriendAction.UnblockFriend: return "unblock";
            default: return string.Empty;
        }
    }
}
