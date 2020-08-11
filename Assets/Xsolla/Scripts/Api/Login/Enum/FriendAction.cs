/// <summary>
/// Type of the action.
/// </summary>
/// <see cref="https://developers.xsolla.com/user-account-api/user-friends/postusersmerelationships"/>
public enum FriendAction
{
    /// <summary>
    /// Send friend request.
    /// </summary>
    SendInviteRequest,
    /// <summary>
    /// Cancel the friend request that was sent.
    /// </summary>
    CancelRequest,
    /// <summary>
    /// Confirm the friend request.
    /// </summary>
    AcceptInvite,
    /// <summary>
    /// Cancel the friend request that was received.
    /// </summary>
    DenyInvite,
    /// <summary>
    /// Delete the user from the friend list.
    /// </summary>
    RemoveFriend,
    /// <summary>
    /// Block the user.
    /// </summary>
    BlockFriend,
    /// <summary>
    /// Unblock the user.
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
            case FriendAction.RemoveFriend: return "friend_request_remove";
            case FriendAction.BlockFriend: return "friend_request_block";
            case FriendAction.UnblockFriend: return "friend_request_unblock";
            default: return string.Empty;
        }
    }
}
