using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Login
{
	public partial class XsollaLogin : MonoSingleton<XsollaLogin>
	{
		private const string URL_USER_GET_FRIENDS = "https://login.xsolla.com/api/users/me/relationships?type={0}&sort_by={1}&sort_order={2}&limit={3}&{4}";
		private const string URL_USER_UPDATE_FRIENDS = "https://login.xsolla.com/api/users/me/relationships?{0}";
		private const string URL_USER_SOCIAL_FRIENDS = "https://login.xsolla.com/api/users/me/social_friends?offset={0}&limit{1}&with_xl_uid{2}";
		private const int USER_FRIENDS_DEFAULT_PAGINATION_LIMIT = 20;
		/// <summary>
		/// Gets user’s friends from a social provider.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get User's Friends</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/methods/users/get-users-friends/"/>.
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="provider">Name of social provider.</param>
		/// <param name="offset">Offset.</param>
		/// <param name="limit">Limit.</param>
		/// <param name="withUid"></param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void GetUserSocialFriends(string token, SocialProvider provider = SocialProvider.None, uint offset = 0, uint limit = 500, bool withUid = false, Action<UserSocialFriends> onSuccess = null, Action<Error> onError = null)
		{
			var urlBuilder = new StringBuilder(
				string.Format(URL_USER_SOCIAL_FRIENDS, offset, limit, withUid ? "true" : "false"));
			if(provider != SocialProvider.None)
				urlBuilder.Append($"&platform={provider.GetParameter()}");
			urlBuilder.Append($"&{AnalyticUrlAddition}");

			var headers = AppendAnalyticHeadersTo(WebRequestHeader.AuthHeader(token));
			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), headers, onSuccess, onError);
		}

		/// <summary>
		/// Gets the friends of the authenticated user.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get Friends</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/user-account-api/user-friends/get-friends"/>.
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="type">Friends type.</param>
		/// <param name="sortType">Condition for sorting users.</param>
		/// <param name="order">Condition for sorting users.</param>
		/// <param name="count">Maximum friends.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void GetUserFriends(
			string token,
			FriendsSearchType type,
			FriendsSearchResultsSort sortType = FriendsSearchResultsSort.ByNickname, 
			FriendsSearchResultsSortOrder order = FriendsSearchResultsSortOrder.Asc,
			int count = 100,
			Action<List<UserFriendEntity>> onSuccess = null, Action<Error> onError = null)
		{
			if (count <= 0)
			{
				Debug.LogError($"Can not requests friends with count {count}");
				return;
			}
			var url = string.Format(URL_USER_GET_FRIENDS, type.GetParameter(), sortType.GetParameter(), order.GetParameter(), USER_FRIENDS_DEFAULT_PAGINATION_LIMIT, AnalyticUrlAddition);
			StartCoroutine(GetUserFriendsCoroutine(token, url, count, onSuccess, onError));
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
					if(friends.Count > count)
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
			var headers = AppendAnalyticHeadersTo(WebRequestHeader.AuthHeader(token));
			WebRequestHelper.Instance.GetRequest(url, headers, onSuccess, onError);
		}

		/// <summary>
		/// Updates the friend list of the authenticated user.
		/// </summary>
		/// <remarks> Swagger method name:<c>Update Friends</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/user-account-api/user-friends/postusersmerelationships"/>.
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="action">Type of the action.</param>
		/// <param name="user">The Xsolla Login user ID to change relationship with.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void UpdateUserFriends(string token, FriendAction action, string user, Action onSuccess, Action<Error> onError)
		{
			var request = new UserFriendUpdate
			{
				action = action.GetParameter(),
				user = user
			};
			var url = string.Format(URL_USER_UPDATE_FRIENDS, AnalyticUrlAddition);
			var headers = AppendAnalyticHeadersTo(WebRequestHeader.AuthHeader(token));
			WebRequestHelper.Instance.PostRequest(url, request, headers, onSuccess, onError);
		}
	}
}