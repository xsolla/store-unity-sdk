using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;
using Xsolla.Login;

public partial class DemoImplementation : MonoBehaviour, IDemoImplementation
{
	/// <summary>
	/// Maximum friends count to display.
	/// </summary>
	private const int MAX_FRIENDS_COUNT = 100;

	private const FriendsSearchResultsSortOrder FRIENDS_SORT_ORDER = FriendsSearchResultsSortOrder.Asc;
	private const FriendsSearchResultsSort FRIENDS_SORT_TYPE = FriendsSearchResultsSort.ByNickname;

	public void GetUserFriends(Action<List<FriendModel>> onSuccess = null, Action<Error> onError = null)
	{
		GetUsersByType(FriendsSearchType.Added, UserRelationship.Friend, onSuccess, onError);
	}

	public void GetBlockedUsers(Action<List<FriendModel>> onSuccess = null, Action<Error> onError = null)
	{
		GetUsersByType(FriendsSearchType.Blocked, UserRelationship.Blocked, onSuccess, onError);
	}

	public void GetPendingUsers(Action<List<FriendModel>> onSuccess = null, Action<Error> onError = null)
	{
		GetUsersByType(FriendsSearchType.Pending, UserRelationship.Pending, onSuccess, onError);
	}

	public void GetRequestedUsers(Action<List<FriendModel>> onSuccess = null, Action<Error> onError = null)
	{
		GetUsersByType(FriendsSearchType.Requested, UserRelationship.Requested, onSuccess, onError);
	}

	public void BlockUser(FriendModel user, Action<FriendModel> onSuccess = null, Action<Error> onError = null)
	{
		PopupFactory.Instance.CreateConfirmation()
			.SetMessage($"Block {user.Nickname}?")
			.SetConfirmButtonText("BLOCK")
			.SetConfirmCallback(() =>
			{
				XsollaLogin.Instance.UpdateUserFriends(XsollaLogin.Instance.Token, FriendAction.BlockFriend, user.Id,
					() => onSuccess?.Invoke(user), onError);
			});
	}

	public void UnblockUser(FriendModel user, Action<FriendModel> onSuccess = null, Action<Error> onError = null)
	{
		XsollaLogin.Instance.UpdateUserFriends(XsollaLogin.Instance.Token, FriendAction.UnblockFriend, user.Id,
			() => onSuccess?.Invoke(user), onError);
	}

	public void SendFriendshipInvite(FriendModel user, Action<FriendModel> onSuccess = null, Action<Error> onError = null)
	{
		XsollaLogin.Instance.UpdateUserFriends(XsollaLogin.Instance.Token, FriendAction.SendInviteRequest, user.Id,
			() => onSuccess?.Invoke(user), onError);
	}

	public void RemoveFriend(FriendModel user, Action<FriendModel> onSuccess = null, Action<Error> onError = null)
	{
		PopupFactory.Instance.CreateConfirmation()
			.SetMessage($"Remove {user.Nickname} from the friend list?")
			.SetConfirmButtonText("REMOVE")
			.SetConfirmCallback(() =>
			{
				XsollaLogin.Instance.UpdateUserFriends(XsollaLogin.Instance.Token, FriendAction.RemoveFriend, user.Id,
					() => onSuccess?.Invoke(user), onError);
			});
	}
	
	public void AcceptFriendship(FriendModel user, Action<FriendModel> onSuccess = null, Action<Error> onError = null)
	{
		XsollaLogin.Instance.UpdateUserFriends(XsollaLogin.Instance.Token, FriendAction.AcceptInvite, user.Id,
			() => onSuccess?.Invoke(user), onError);
	}
	
	public void DeclineFriendship(FriendModel user, Action<FriendModel> onSuccess = null, Action<Error> onError = null)
	{
		XsollaLogin.Instance.UpdateUserFriends(XsollaLogin.Instance.Token, FriendAction.DenyInvite, user.Id,
			() => onSuccess?.Invoke(user), onError);
	}
	
	public void CancelFriendshipRequest(FriendModel user, Action<FriendModel> onSuccess = null, Action<Error> onError = null)
	{
		XsollaLogin.Instance.UpdateUserFriends(XsollaLogin.Instance.Token, FriendAction.CancelRequest, user.Id,
			() => onSuccess?.Invoke(user), onError);
	}

	private void GetUsersByType(FriendsSearchType searchType, UserRelationship relationship,
		Action<List<FriendModel>> onSuccess = null, Action<Error> onError = null)
	{
		XsollaLogin.Instance.GetUserFriends(XsollaLogin.Instance.Token,
			searchType, FRIENDS_SORT_TYPE, FRIENDS_SORT_ORDER, MAX_FRIENDS_COUNT, friends =>
			{
				onSuccess?.Invoke(friends.Select(f =>
				{
					var result = ConvertFriendEntity(f, relationship);
					// this method used at this place for fastest image loading
					ImageLoader.Instance.GetImageAsync(result.AvatarUrl, null);
					return result;
				}).ToList());
			}, WrapErrorCallback(onError));
	}

	private FriendModel ConvertFriendEntity(UserFriendEntity friend, UserRelationship relationship)
	{
		return new FriendModel
		{
			Id = friend.user.id,
			Nickname = GetUserNickname(friend),
			AvatarUrl = friend.user.picture,
			Status = friend.IsOnline() ? UserOnlineStatus.Online : UserOnlineStatus.Offline,
			Relationship = relationship
		};
	}

	private string GetUserNickname(UserFriendEntity friend)
	{
		return !string.IsNullOrEmpty(friend.user.nickname) 
			? friend.user.nickname 
			: (!string.IsNullOrEmpty(friend.user.name) 
				? friend.user.name 
				: (!string.IsNullOrEmpty(friend.user.first_name) 
					? friend.user.first_name 
					: (!string.IsNullOrEmpty(friend.user.last_name) 
						? friend.user.last_name 
						: (!string.IsNullOrEmpty(friend.user.email) 
							? friend.user.email 
							: "User without name"))));
	}
}
