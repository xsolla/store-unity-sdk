using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.UserAccount
{
	public partial class XsollaUserAccount : MonoSingleton<XsollaUserAccount>
	{
		private const string URL_USER_GET_FRIENDS = "https://login.xsolla.com/api/users/me/relationships?type={0}&sort_by={1}&sort_order={2}{3}{4}";
		private const string URL_USER_UPDATE_FRIENDS = "https://login.xsolla.com/api/users/me/relationships";
		private const string URL_USER_SOCIAL_FRIENDS = "https://login.xsolla.com/api/users/me/social_friends?offset={0}&limit={1}&with_xl_uid={2}{3}";
		private const string URL_USER_UPDATE_SOCIAL_FRIENDS = "https://login.xsolla.com/api/users/me/social_friends/update{0}";
		private const int USER_FRIENDS_DEFAULT_PAGINATION_LIMIT = 20;

		/// <summary>
		/// Gets a list of user’s friends from a social provider.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get Social Account Friends</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/methods/users/get-users-friends/"/>.
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="platform">Name of the chosen social provider which you can enable in your Publisher Account > your Login project > Social connections. If you do not specify it, the call gets friends from all social providers.</param>
		/// <param name="offset">Number of the elements from which the list is generated. Default: 0.</param>
		/// <param name="limit">Maximum number of friends that are returned at a time. Default: 500.</param>
		/// <param name="withXlUid">Shows whether the social friends are from your game. Default: false.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void GetUserSocialFriends(string token, SocialProvider platform = SocialProvider.None, uint offset = 0, uint limit = 500, bool withXlUid = false, Action<UserSocialFriends> onSuccess = null, Action<Error> onError = null)
		{
			var withUidFlag = withXlUid ? "true" : "false";
			var providerUrlAddition = platform != SocialProvider.None ? $"&platform={platform.GetParameter()}" : string.Empty;
			var url = string.Format(URL_USER_SOCIAL_FRIENDS, offset, limit, withUidFlag, providerUrlAddition);

			WebRequestHelper.Instance.GetRequest(SdkType.Login, url, WebRequestHeader.AuthHeader(token), onSuccess, onError);
		}

		/// <summary>
		/// Begins data processing to update a list of user’s friends from a social provider.
		/// Note that there may be a delay in data processing because of the Xsolla Login server or provider server high loads.
		/// </summary>
		/// <remarks> Swagger method name:<c>Update Social Account Friends</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/methods/users/update-users-friends/"/>.
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="platform">Name of the chosen social provider which you can enable in your Publisher Account > your Login project > Social connections.
		/// If you do not specify it, the call gets friends from all social providers.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void UpdateUserSocialFriends(string token, SocialProvider platform = SocialProvider.None, Action onSuccess = null, Action<Error> onError = null)
		{
			var providerUrlAddition = platform != SocialProvider.None ? $"?platform={platform.GetParameter()}" : string.Empty;
			var url = string.Format(URL_USER_UPDATE_SOCIAL_FRIENDS, providerUrlAddition);

			WebRequestHelper.Instance.PostRequest(SdkType.Login, url, WebRequestHeader.AuthHeader(token), onSuccess, onError);
		}

		/// <summary>
		/// Gets a list of users added as friends of the authenticated user.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get Friends</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/user-account-api/user-friends/get-friends"/>.
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="type">Friends type.</param>
		/// <param name="sortBy">Condition for sorting the users.</param>
		/// <param name="sortOrder">Condition for sorting the list of users.</param>
		/// <param name="after">Parameter that is used for API pagination.</param>
		/// <param name="limit">Maximum number of users that are returned at a time. Default: 20.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void GetUserFriends(
			string token,
			FriendsSearchType type,
			FriendsSearchResultsSort sortBy = FriendsSearchResultsSort.ByNickname,
			FriendsSearchResultsSortOrder sortOrder = FriendsSearchResultsSortOrder.Asc,
			string after = null,
			int? limit = null,
			Action<List<UserFriendEntity>> onSuccess = null, Action<Error> onError = null)
		{
			var afterParam = (after != null) ? $"&after={after}" : "";
			var limitParam = default(string);

			if (limit.HasValue)
			{
				if (limit.Value < 1 || limit.Value > 50)
				{
					Debug.LogError($"Limit must be 1-50. Default limit is 20. Current limit:{limit.Value}");
					return;
				}

				limitParam = $"&limit={limit.Value}";
			}
			else
			{
				limit = USER_FRIENDS_DEFAULT_PAGINATION_LIMIT;
				limitParam = "";
			}

			var url = string.Format(URL_USER_GET_FRIENDS, type.GetParameter(), sortBy.GetParameter(), sortOrder.GetParameter(), afterParam, limitParam);
			StartCoroutine(GetUserFriendsCoroutine(token, url, limit.Value, onSuccess, onError));
		}

		IEnumerator GetUserFriendsCoroutine(string token, string url, int count, Action<List<UserFriendEntity>> onSuccess, Action<Error> onError)
		{
			var result = new List<UserFriendEntity>();
			while (count > 0 && !string.IsNullOrEmpty(url))
			{
				var busy = true;
				GetUserFriendsInternal(token, url, response =>
				{
					var friends = response.relationships;
					if (friends.Count > count)
						friends = friends.Take(count).ToList();
					result.AddRange(friends);
					count -= friends.Count;
					url = response.next_url;
					busy = false;
				}, error =>
				{
					onError?.Invoke(error);
					url = string.Empty;
					busy = false;
				});
				yield return new WaitWhile(() => busy);
			}
			onSuccess?.Invoke(result);
		}

		private void GetUserFriendsInternal(string token, string url, Action<UserFriendsEntity> onSuccess = null, Action<Error> onError = null)
		{
			WebRequestHelper.Instance.GetRequest(SdkType.Login, url, WebRequestHeader.AuthHeader(token), onSuccess, onError);
		}

		/// <summary>
		/// Updates the friend list of the authenticated user.
		/// </summary>
		/// <remarks> Swagger method name:<c>Update User's Friends</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/user-account/managed-by-client/user-friends/update-users-friends"/>.
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="action">Type of the action.</param>
		/// <param name="user">ID of the user to change relationship with.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void UpdateUserFriends(string token, FriendAction action, string user, Action onSuccess, Action<Error> onError)
		{
			var request = new UserFriendUpdate
			{
				action = action.GetParameter(),
				user = user
			};
			WebRequestHelper.Instance.PostRequest(SdkType.Login, URL_USER_UPDATE_FRIENDS, request, WebRequestHeader.AuthHeader(token), onSuccess, onError);
		}
	}
}
