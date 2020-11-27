using System;
using System.Collections;
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

	public void ForceUpdateFriendsFromSocialNetworks(Action onSuccess = null, Action<Error> onError = null)
	{
		XsollaLogin.Instance.UpdateUserSocialFriends(XsollaLogin.Instance.Token, SocialProvider.None, onSuccess, onError);
	}

	public void GetFriendsFromSocialNetworks(Action<List<FriendModel>> onSuccess = null, Action<Error> onError = null)
	{
		XsollaLogin.Instance.GetUserSocialFriends(XsollaLogin.Instance.Token, SocialProvider.None, 0, 20, false,
			onSuccess: friends => StartCoroutine(ConvertSocialFriendsToRecommended(friends.data, onSuccess, onError)),
			onError);
	}

	private IEnumerator ConvertSocialFriendsToRecommended(List<UserSocialFriend> socialFriends, Action<List<FriendModel>> onSuccess = null, Action<Error> onError = null)
	{
		var recommendedFriends = new List<FriendModel>(socialFriends.Count);

		foreach (var socialFriend in socialFriends)
		{
			var recommendedFriend = ConvertFriendEntity(socialFriend);

			if (recommendedFriend.Relationship == UserRelationship.SocialNonXsolla)//Social friend without linked Xsolla account
			{
				//Proceed
			}
			else if (recommendedFriend.Relationship != UserRelationship.Unknown)//User is already added/blocked/requested
			{
				continue;//Skip this user, they should not appear as recommended
			}
			else//Social friend with linked Xsolla account, replace nickname and avatar with ones from Xsolla
			{
				var token = DemoController.Instance.GetImplementation().Token;
				bool? isUserinfoObtained = null;

				DemoController.Instance.GetImplementation().GetPublicInfo(token, recommendedFriend.Id,
					onSuccess: info =>
					{
						recommendedFriend.Nickname = info.nickname;
						recommendedFriend.AvatarUrl = info.avatar;
						isUserinfoObtained = true;
					},
					onError: error =>
					{
						onError?.Invoke(error);
						isUserinfoObtained = false;
					});

				yield return new WaitWhile(() => isUserinfoObtained == null);

				if (isUserinfoObtained == false)
				{
					Debug.LogError($"Could not get user information. UserID: {recommendedFriend.Id}");
					yield break;
				}
			}

			recommendedFriends.Add(recommendedFriend);

			//Avatar preload
			if (!string.IsNullOrEmpty(recommendedFriend.AvatarUrl))
				ImageLoader.Instance.GetImageAsync(recommendedFriend.AvatarUrl, null);
		}

		onSuccess?.Invoke(recommendedFriends);
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
					if(!string.IsNullOrEmpty(result.AvatarUrl))
						ImageLoader.Instance.GetImageAsync(result.AvatarUrl, null);
					return result;
				}).ToList());
			}, WrapErrorCallback(onError));
	}

	private FriendModel ConvertFriendEntity(UserSocialFriend friend)
	{
		var result = new FriendModel
		{
			Id = friend.xl_uid,
			Nickname = friend.name,
			AvatarUrl = friend.avatar
		};

		if (!string.IsNullOrEmpty(friend.xl_uid))//Xsolla ID not null - this is a registered Xsolla user with linked social account
		{
			var existingFriend = UserFriends.Instance.GetUserById(friend.xl_uid);
			result.Status = existingFriend?.Status ?? UserOnlineStatus.Unknown;
			result.Relationship = existingFriend?.Relationship ?? UserRelationship.Unknown;
		}
		else
		{
			result.Status = UserOnlineStatus.Unknown;
			result.Relationship = UserRelationship.SocialNonXsolla;
		}

		if (Enum.TryParse<SocialProvider>(friend.platform, ignoreCase: true, out SocialProvider provider))
			result.SocialProvider = provider;

		return result;
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
