using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Login;

namespace Xsolla.Demo
{
    public class SdkFriendsLogic : MonoSingleton<SdkFriendsLogic>
	{
		/// <summary>
		/// Maximum friends count to display.
		/// </summary>
		private const int MAX_FRIENDS_COUNT = 100;
		/// <summary>
		/// Default friend name value if the name isn't provided.
		/// </summary>
		private const string DEFAULT_NAME_VALUE = "User without name";

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
			XsollaLogin.Instance.UpdateUserFriends(Token.Instance, FriendAction.BlockFriend, user.Id,
				() =>
				{
					if (onSuccess != null)
						onSuccess.Invoke(user);
				},
				onError);
		}

		public void UnblockUser(FriendModel user, Action<FriendModel> onSuccess = null, Action<Error> onError = null)
		{
			XsollaLogin.Instance.UpdateUserFriends(Token.Instance, FriendAction.UnblockFriend, user.Id,
				() =>
				{
					if (onSuccess != null)
						onSuccess.Invoke(user);
				},
				onError);
		}

		public void SendFriendshipInvite(FriendModel user, Action<FriendModel> onSuccess = null, Action<Error> onError = null)
		{
			XsollaLogin.Instance.UpdateUserFriends(Token.Instance, FriendAction.SendInviteRequest, user.Id,
				() =>
				{
					if (onSuccess != null)
						onSuccess.Invoke(user);
				},
				onError);
		}

		public void RemoveFriend(FriendModel user, Action<FriendModel> onSuccess = null, Action<Error> onError = null)
		{
			XsollaLogin.Instance.UpdateUserFriends(Token.Instance, FriendAction.RemoveFriend, user.Id,
				() =>
				{
					if (onSuccess != null)
						onSuccess.Invoke(user);
				},
				onError);
		}

		public void AcceptFriendship(FriendModel user, Action<FriendModel> onSuccess = null, Action<Error> onError = null)
		{
			XsollaLogin.Instance.UpdateUserFriends(Token.Instance, FriendAction.AcceptInvite, user.Id,
				() =>
				{
					if (onSuccess != null)
						onSuccess.Invoke(user);
				},
				onError);
		}

		public void DeclineFriendship(FriendModel user, Action<FriendModel> onSuccess = null, Action<Error> onError = null)
		{
			XsollaLogin.Instance.UpdateUserFriends(Token.Instance, FriendAction.DenyInvite, user.Id,
				() =>
				{
					if (onSuccess != null)
						onSuccess.Invoke(user);
				},
				onError);
		}

		public void CancelFriendshipRequest(FriendModel user, Action<FriendModel> onSuccess = null, Action<Error> onError = null)
		{
			XsollaLogin.Instance.UpdateUserFriends(Token.Instance, FriendAction.CancelRequest, user.Id,
				() =>
				{
					if (onSuccess != null)
						onSuccess.Invoke(user);
				},
				onError);
		}

		public void ForceUpdateFriendsFromSocialNetworks(Action onSuccess = null, Action<Error> onError = null)
		{
			XsollaLogin.Instance.UpdateUserSocialFriends(Token.Instance, SocialProvider.None, onSuccess, onError);
		}

		public void GetFriendsFromSocialNetworks(Action<List<FriendModel>> onSuccess = null, Action<Error> onError = null)
		{
			XsollaLogin.Instance.GetUserSocialFriends(Token.Instance, SocialProvider.None, 0, 500, false,
				friends => StartCoroutine(ConvertSocialFriendsToRecommended(friends.data, onSuccess, onError)),
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
					var token = Token.Instance;
					bool? isUserinfoObtained = null;

					SdkLoginLogic.Instance.GetPublicInfo(token, recommendedFriend.Id,
						onSuccess: info =>
						{
							recommendedFriend.Nickname = info.nickname;
							recommendedFriend.AvatarUrl = info.avatar;
							isUserinfoObtained = true;
						},
						onError: error =>
						{
							if (onError != null)
								onError.Invoke(error);
							isUserinfoObtained = false;
						});

					yield return new WaitWhile(() => isUserinfoObtained == null);

					if (isUserinfoObtained == false)
					{
						Debug.LogError(string.Format("Could not get user information. UserID: {0}", recommendedFriend.Id));
						yield break;
					}
				}

				recommendedFriends.Add(recommendedFriend);

				//Avatar preload
				if (!string.IsNullOrEmpty(recommendedFriend.AvatarUrl))
					ImageLoader.Instance.GetImageAsync(recommendedFriend.AvatarUrl, null);
			}

			if (onSuccess != null)
				onSuccess.Invoke(recommendedFriends);
		}

		private void GetUsersByType(FriendsSearchType searchType, UserRelationship relationship,
			Action<List<FriendModel>> onSuccess = null, Action<Error> onError = null)
		{
			XsollaLogin.Instance.GetUserFriends(Token.Instance,
				searchType, FRIENDS_SORT_TYPE, FRIENDS_SORT_ORDER, MAX_FRIENDS_COUNT, friends =>
				{
					if (onSuccess != null)
						onSuccess.Invoke(friends.Select(f =>
						{
							var result = ConvertFriendEntity(f, relationship);
							// this method used at this place for fastest image loading
							if (!string.IsNullOrEmpty(result.AvatarUrl))
								ImageLoader.Instance.GetImageAsync(result.AvatarUrl, null);
							return result;
						}).ToList());
				}, onError);
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

				if (existingFriend != null)
				{
					result.Status = existingFriend.Status;
					result.Relationship = existingFriend.Relationship;
				}
				else
				{
					result.Status = UserOnlineStatus.Unknown;
					result.Relationship = UserRelationship.Unknown;
				}
			}
			else
			{
				result.Status = UserOnlineStatus.Unknown;
				result.Relationship = UserRelationship.SocialNonXsolla;
			}

			try
			{
				result.SocialProvider = (SocialProvider)Enum.Parse(typeof(SocialProvider), friend.platform);
			}
			catch (Exception)
			{
				result.SocialProvider = SocialProvider.None;
			}

			return result;
		}

		private FriendModel ConvertFriendEntity(UserFriendEntity friend, UserRelationship relationship)
		{
			return new FriendModel
			{
				Id = friend.user.id,
				Nickname = GetUserNickname(friend),
				Tag = friend.user.tag,
				AvatarUrl = friend.user.picture,
				Status = friend.IsOnline() ? UserOnlineStatus.Online : UserOnlineStatus.Offline,
				Relationship = relationship
			};
		}

		private string GetUserNickname(UserFriendEntity friend)
		{
			var userInfo = friend.user;

			if (!string.IsNullOrEmpty(userInfo.nickname))
				return userInfo.nickname;
			if (!string.IsNullOrEmpty(userInfo.name))
				return userInfo.name;
			if (!string.IsNullOrEmpty(userInfo.first_name))
				return userInfo.first_name;
			if (!string.IsNullOrEmpty(userInfo.last_name))
				return userInfo.last_name;
			if (!string.IsNullOrEmpty(userInfo.email))
				return userInfo.email;
			else
				return DEFAULT_NAME_VALUE;
		}
	}
}
