using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Login;

namespace Xsolla.Demo
{
    public class SdkLoginLogic : MonoSingleton<SdkLoginLogic>
	{
		private const int NETWORKS_CACHE_TIMEOUT = 5;

	#region Token
		public void ValidateToken(string token, Action<string> onSuccess = null, Action<Error> onError = null)
		{
			GetUserInfo(token, useCache: false, onSuccess: info => onSuccess?.Invoke(token), onError: onError);
		}
	#endregion

	#region User
		public void GetUserInfo(string token, Action<UserInfo> onSuccess = null, Action<Error> onError = null)
		{
			GetUserInfo(token, useCache: true, onSuccess, onError);
		}

		private readonly Dictionary<string, UserInfo> _userCache = new Dictionary<string, UserInfo>();
		public void GetUserInfo(string token, bool useCache, Action<UserInfo> onSuccess, Action<Error> onError = null)
		{
			if (useCache && _userCache.ContainsKey(token))
				onSuccess?.Invoke(_userCache[token]);
			else
				XsollaLogin.Instance.GetUserInfo(token, info =>
				{
					_userCache[token] = info;
					onSuccess?.Invoke(info);
				}, onError);
		}

		public void UpdateUserInfo(string token, UserInfoUpdate info, Action<UserInfo> onSuccess, Action<Error> onError = null)
		{
			Action<UserInfo> successCallback = newInfo =>
			{
				_userCache[token] = newInfo;
				onSuccess?.Invoke(newInfo);
			};

			XsollaLogin.Instance.UpdateUserInfo(token, info, successCallback, onError);

		}

		public void GetPublicInfo(string token, string user, Action<UserPublicInfo> onSuccess, Action<Error> onError = null)
		{
			XsollaLogin.Instance.GetPublicInfo(token, user, onSuccess, onError = null);
		}

		public void Registration(string username, string password, string email, string state = null, Action onSuccess = null, Action<Error> onError = null)
		{
			XsollaLogin.Instance.Registration(username, password, email, state, null, true, true, null, onSuccess, onError);
		}

		public void SignIn(string username, string password, bool rememberUser, Action<string> onSuccess, Action<Error> onError = null)
		{
			XsollaLogin.Instance.SignIn(username, password, rememberUser, null, onSuccess, onError);
		}

		public void AccessTokenAuth(string email, Action onSuccess, Action<Error> onError = null)
		{
			var authParams = new AccessTokenAuthParams()
			{
				parameters = new Dictionary<string, object>() { { "email", email } }
			};

			XsollaLogin.Instance.GetUserAccessToken(authParams, onSuccess, onError);
		}

		public void ResetPassword(string username, Action onSuccess = null, Action<Error> onError = null)
		{
			XsollaLogin.Instance.ResetPassword(username, onSuccess, onError);
		}

		public void ResendConfirmationLink(string username, Action onSuccess = null, Action<Error> onError = null)
		{
			XsollaLogin.Instance.ResendConfirmationLink(username, null, null, onSuccess, onError);
		}

		public void ChangeUserPhoneNumber(string token, string phoneNumber, Action onSuccess, Action<Error> onError)
		{
			XsollaLogin.Instance.ChangeUserPhoneNumber(token, phoneNumber, onSuccess, onError);
		}

		public void DeleteUserPhoneNumber(string token, string phoneNumber, Action onSuccess, Action<Error> onError)
		{
			XsollaLogin.Instance.DeleteUserPhoneNumber(token, phoneNumber, onSuccess, onError);
		}

		public void SearchUsersByNickname(string nickname, Action<List<FriendModel>> onSuccess = null, Action<Error> onError = null)
		{
			nickname = Uri.EscapeDataString(nickname);

			XsollaLogin.Instance.SearchUsers(Token.Instance, nickname, 0, 20,
				onSuccess: users =>
				{
					onSuccess?.Invoke(users.users.Where(u => !u.is_me).Select(u =>
					{
						var result = new FriendModel
						{
							Id = u.user_id,
							AvatarUrl = u.avatar,
							Nickname = u.nickname,
							Tag = u.tag
						};
						var user = UserFriends.Instance.GetUserById(result.Id);
						result.Status = user?.Status ?? UserOnlineStatus.Unknown;
						result.Relationship = user?.Relationship ?? UserRelationship.Unknown;
						return result;
					}).ToList());
				},
				onError: onError);
		}
	#endregion

	#region Social
		public void SteamAuth(string appId, string sessionTicket, string state = null, Action<string> onSuccess = null, Action<Error> onError = null)
		{
			XsollaLogin.Instance.SteamAuth(appId, sessionTicket, state, null, onSuccess, onError);
		}

		public void AuthViaDeviceID(Core.DeviceType deviceType, string deviceName, string deviceId, string payload = null, string state = null, Action<string> onSuccess = null, Action<Error> onError = null)
		{
			XsollaLogin.Instance.AuthViaDeviceID(deviceType, deviceName, deviceId, payload, state, onSuccess, onError);
		}

		public string GetSocialNetworkAuthUrl(SocialProvider socialProvider)
		{
			return XsollaLogin.Instance.GetSocialNetworkAuthUrl(socialProvider);
		}

		public void LinkSocialProvider(SocialProvider socialProvider, Action<SocialProvider> onSuccess, Action<Error> onError = null)
		{
			if (EnvironmentDefiner.IsStandaloneOrEditor)
			{
				XsollaLogin.Instance.LinkSocialProvider(
					socialProvider,
					url =>
					{
						var browser = BrowserHelper.Instance.InAppBrowser;
						if (browser != null)
						{
							browser.Open(url);
							
							browser.AddInitHandler(() =>
							{
								browser.AddUrlChangeHandler(newUrl =>
								{
									Debug.Log($"URL = {newUrl}");
									if (ParseUtils.TryGetValueFromUrl(newUrl, ParseParameter.token, out _))
									{
										browser.Close();
										HotkeyCoroutine.Unlock(); 
										onSuccess?.Invoke(socialProvider);
									}
								});
							});
							
							browser.AddCloseHandler(() =>
							{
								onError?.Invoke(null);
							});
						}
						else
						{
							Debug.LogError("This functionality is not supported elsewhere except Editor and Standalone build");
							onError?.Invoke(null);
						}
					}, 
					onError);
			}
			else
			{
				var errorMessage = "LinkSocialProvider: This functionality is not supported elswere except Editor and Standalone build";
				Debug.LogError(errorMessage);
				onError?.Invoke(new Error(ErrorType.MethodIsNotAllowed, errorMessage: errorMessage));
			}
		}

		private List<LinkedSocialNetwork> _networksCache;
		private DateTime _networksCacheTime;
		private bool _networksCacheInProgress;

		public void GetLinkedSocialProviders(Action<List<LinkedSocialNetwork>> onSuccess, Action<Error> onError = null)
		{
			if (_networksCacheInProgress)
			{
				StartCoroutine(WaitLinkedSocialProviders(onSuccess));
				return;
			}
			if ((DateTime.Now - _networksCacheTime).Seconds > NETWORKS_CACHE_TIMEOUT || _networksCache == null)
			{
				_networksCacheInProgress = true;
				XsollaLogin.Instance.GetLinkedSocialProviders(networks =>
				{
					_networksCache = networks;
					_networksCacheTime = DateTime.Now;
					onSuccess?.Invoke(_networksCache);
					_networksCacheInProgress = false;
				}, error =>
				{
					if (_networksCache == null)
						_networksCache = new List<LinkedSocialNetwork>();
					onError?.Invoke(error);
					_networksCacheInProgress = false;
				});
			}
			else
			{
				onSuccess?.Invoke(_networksCache);
			}
		}

		private IEnumerator WaitLinkedSocialProviders(Action<List<LinkedSocialNetwork>> onSuccess)
		{
			yield return new WaitWhile(() => _networksCacheInProgress);
			onSuccess?.Invoke(_networksCache);
		}

	#endregion

	#region AccountLinking
		public void SignInConsoleAccount(string userId, string platform, Action<string> successCase, Action<Error> failedCase)
		{
			XsollaLogin.Instance.SignInConsoleAccount(userId, platform, successCase, failedCase);
		}

		public void RequestLinkingCode(Action<LinkingCode> onSuccess, Action<Error> onError)
		{
			XsollaLogin.Instance.RequestLinkingCode(onSuccess, onError);
		}

		public void LinkConsoleAccount(string userId, string platform, string confirmationCode, Action onSuccess, Action<Error> onError)
		{
			XsollaLogin.Instance.LinkConsoleAccount(userId, platform, confirmationCode, onSuccess, onError);
		}
	#endregion

	#region Attributes
		public void GetUserAttributes(string token, string projectId, UserAttributeType attributeType,
			List<string> attributeKeys, string userId, Action<List<UserAttribute>> onSuccess, Action<Error> onError)
		{
			XsollaLogin.Instance.GetUserAttributes(token, projectId, attributeType, attributeKeys, userId, onSuccess, onError);
		}

		public void UpdateUserAttributes(string token, string projectId, List<UserAttribute> attributes, Action onSuccess, Action<Error> onError)
		{
			XsollaLogin.Instance.UpdateUserAttributes(token, projectId, attributes, onSuccess, onError);
		}

		public void RemoveUserAttributes(string token, string projectId, List<string> attributeKeys, Action onSuccess, Action<Error> onError)
		{
			XsollaLogin.Instance.RemoveUserAttributes(token, projectId, attributeKeys, onSuccess, onError);
		}
	#endregion

	#region OAuth2.0
		public bool IsOAuthTokenRefreshInProgress => XsollaLogin.Instance.IsOAuthTokenRefreshInProgress;

		public void ExchangeCodeToToken(string code, Action<string> onSuccessExchange = null, Action<Error> onError = null)
		{
			XsollaLogin.Instance.ExchangeCodeToToken(code, onSuccessExchange, onError);
		}
	#endregion

	#region Picture
		public void UploadUserPicture(string token, byte[] pictureData, string boundary, Action<string> onSuccess, Action<Error> onError)
		{
			XsollaLogin.Instance.UploadUserPicture(token, pictureData, boundary, onSuccess, onError);
		}

		public void DeleteUserPicture(string token, Action onSuccess, Action<Error> onError)
		{
			XsollaLogin.Instance.DeleteUserPicture(token, onSuccess, onError);
		}
	#endregion
	}
}
