using System;
using System.Linq;
using System.Text;
using Xsolla.Core;

namespace Xsolla.Login
{
	public partial class XsollaLogin : MonoSingleton<XsollaLogin>
	{
		private const string URL_USER_GET_FRIENDS = "https://login.xsolla.com/api/users/me/relationships?type{0}&sort_by={1}&sort_order={2}";
		private const string URL_USER_UPDATE_FRIENDS = "https://login.xsolla.com/api/users/me/relationships";
		private const string URL_USER_SOCIAL_FRIENDS = "https://login.xsolla.com/api/users/me/social_friends?offset={0}&limit{1}&with_xl_uid{2}";

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
			urlBuilder.Append(AdditionalUrlParams);
			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), WebRequestHeader.AuthHeader(token), onSuccess, onError);
		}

		/// <summary>
		/// Gets the friends of the authenticated user.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get Friends</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/user-account-api/user-friends/getusersmerelationships"/>.
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="type">Friends type.</param>
		/// <param name="sortType">Condition for sorting users.</param>
		/// <param name="order">Condition for sorting users.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void GetUserFriends(
			string token,
			FriendsSearchType type,
			FriendsSearchResultsSort sortType = FriendsSearchResultsSort.ByName, 
			FriendsSearchResultsSortOrder order = FriendsSearchResultsSortOrder.Asc, 
			Action<UserFriends> onSuccess = null, Action<Error> onError = null)
		{
			var urlBuilder = new StringBuilder(
				string.Format(URL_USER_GET_FRIENDS, type.GetParameter(), sortType.GetParameter(), order.GetParameter()));
			urlBuilder.Append(AdditionalUrlParams);
			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), WebRequestHeader.AuthHeader(token), onSuccess, onError);
		}

		/// <summary>
		/// Updates the friend list of the authenticated user.
		/// </summary>
		/// <remarks> Swagger method name:<c>Update Friends</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/user-account-api/user-friends/getusersmerelationships"/>.
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="action">Type of the action.</param>
		/// <param name="user">The Xsolla Login user ID to change relationship with.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void UpdateUserFriends(string token, FriendAction action, string user, Action<UserFriendUpdateResponse> onSuccess, Action<Error> onError)
		{
			var request = new UserFriendUpdate
			{
				action = action.GetParameter(),
				user = user
			};
			var urlBuilder = new StringBuilder(URL_USER_UPDATE_FRIENDS);
			urlBuilder.Append("?" + AdditionalUrlParams.TrimStart('&'));
			WebRequestHelper.Instance.PostRequest(urlBuilder.ToString(), request, WebRequestHeader.AuthHeader(token), onSuccess, onError);
		}
	}
}