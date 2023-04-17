namespace Xsolla.UserAccount
{
	/// <summary>
	/// Type of the action.
	/// </summary>
	/// <see href="https://developers.xsolla.com/user-account-api/user-friends/postusersmerelationships"/>
	public enum FriendAction
	{
		/// <summary>
		/// To send a friend request
		/// </summary>
		SendInviteRequest,
		/// <summary>
		/// To cancel the friend request that was sent
		/// </summary>
		CancelRequest,
		/// <summary>
		/// To confirm the friend request
		/// </summary>
		AcceptInvite,
		/// <summary>
		/// To cancel the friend request that was received
		/// </summary>
		DenyInvite,
		/// <summary>
		/// To delete the user from the friend list
		/// </summary>
		RemoveFriend,
		/// <summary>
		/// To block the user
		/// </summary>
		BlockFriend,
		/// <summary>
		/// To unblock the user
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
}
