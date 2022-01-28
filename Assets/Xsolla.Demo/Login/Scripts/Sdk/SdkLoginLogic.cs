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

		public event Action RegistrationEvent;
		public event Action LoginEvent;

		public event Action UpdateUserInfoEvent;
		
		public event Action UpdateUserAttributesEvent;
		public event Action RemoveUserAttributesEvent;

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
			Action<UserInfo> successCallback = userInfo =>
			{
				_userCache[token] = userInfo;
				onSuccess?.Invoke(userInfo);
				UpdateUserInfoEvent?.Invoke();
			};

			XsollaLogin.Instance.UpdateUserInfo(token, info, successCallback, onError);

		}

		public void GetPublicInfo(string token, string user, Action<UserPublicInfo> onSuccess, Action<Error> onError = null)
		{
			XsollaLogin.Instance.GetPublicInfo(token, user, onSuccess, onError = null);
		}

		public void Registration(string username, string password, string email, string state = null, Action onSuccess = null, Action<Error> onError = null)
		{
			Action successCallback = () =>
			{
				onSuccess?.Invoke();
				RegistrationEvent?.Invoke();
			};
			
			XsollaLogin.Instance.Registration(username, password, email, null, state, null, true, true, null, successCallback, onError);
		}
		
		public void Registration(string username, string password, string email, string state = null, Action<int> onSuccess = null, Action<Error> onError = null)
		{
			Action<int> successCallback = responseCode =>
			{
				onSuccess?.Invoke(responseCode);
				RegistrationEvent?.Invoke();
			};
			
			XsollaLogin.Instance.Registration(username, password, email, null, state, null, true, true, null, successCallback, onError);
		}

		public void Registration(string username, string password, string email, string state = null, Action<LoginUrlResponse> onSuccess = null, Action<Error> onError = null)
		{
			Action<LoginUrlResponse> successCallback = response =>
			{
				onSuccess?.Invoke(response);
				RegistrationEvent?.Invoke();
			};
			
			XsollaLogin.Instance.Registration(username, password, email, null, state, null, true, true, null, successCallback, onError);
		}

		public void SignIn(string username, string password, bool rememberUser, Action<string> onSuccess, Action<Error> onError = null)
		{
			Action<string> successCallback = token =>
			{
				onSuccess?.Invoke(token);
				LoginEvent?.Invoke();
			};
			
			XsollaLogin.Instance.SignIn(username, password, rememberUser, null, null, successCallback, onError);
		}

		public void AccessTokenAuth(string email, Action onSuccess, Action<Error> onError = null)
		{
			var authParams = new AccessTokenAuthParams()
			{
				parameters = new Dictionary<string, object>() { { "email", email } }
			};
			
			Action successCallback = () =>
			{
				onSuccess?.Invoke();
				LoginEvent?.Invoke();
			};

			XsollaLogin.Instance.GetUserAccessToken(authParams, successCallback, onError);
		}

		public void ResetPassword(string username, Action onSuccess = null, Action<Error> onError = null)
		{
			XsollaLogin.Instance.ResetPassword(username, null, onSuccess, onError);
		}

		public void ResendConfirmationLink(string username, Action onSuccess = null, Action<Error> onError = null)
		{
			XsollaLogin.Instance.ResendConfirmationLink(username, null, null, null, onSuccess, onError);
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
			Action<string> successCallback = token =>
			{
				onSuccess?.Invoke(token);
				LoginEvent?.Invoke();
			};
			
			XsollaLogin.Instance.SteamAuth(appId: appId, sessionTicket: sessionTicket, oauthState: state, payload: null, onSuccess: successCallback, onError: onError);
		}

		public void AuthViaDeviceID(Core.DeviceType deviceType, string deviceName, string deviceId, string payload = null, string state = null, Action<string> onSuccess = null, Action<Error> onError = null)
		{
			Action<string> successCallback = token =>
			{
				onSuccess?.Invoke(token);
				LoginEvent?.Invoke();
			};
			
			XsollaLogin.Instance.AuthViaDeviceID(deviceType, deviceName, deviceId, payload, state, successCallback, onError);
		}

		public string GetSocialNetworkAuthUrl(SocialProvider socialProvider)
		{
			return XsollaLogin.Instance.GetSocialNetworkAuthUrl(socialProvider);
		}

		public void LinkSocialProvider(SocialProvider socialProvider, Action<SocialProvider> onSuccess, Action<Error> onError = null)
		{
			if (!EnvironmentDefiner.IsStandaloneOrEditor)
			{
				var errorMessage = "LinkSocialProvider: This functionality is not supported elswere except Editor and Standalone build";
				Debug.LogError(errorMessage);
				onError?.Invoke(new Error(ErrorType.MethodIsNotAllowed, errorMessage: errorMessage));
				return;
			}

			Action<string> urlCallback = url =>
			{
				var browser = BrowserHelper.Instance.InAppBrowser;
				if (browser == null)
				{
					var message = "LinkSocialProvider: Can not obtain in-built browser";
					Debug.LogError(message);
					onError?.Invoke(new Error(ErrorType.MethodIsNotAllowed, errorMessage: message));
					return;
				}

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
							return;
						}

						if (ParseUtils.TryGetValueFromUrl(newUrl, ParseParameter.error_code, out string errorCode) &&
							ParseUtils.TryGetValueFromUrl(newUrl, ParseParameter.error_description, out string errorDescription))
						{
							browser.Close();
							HotkeyCoroutine.Unlock();
							onError?.Invoke(new Error(statusCode: errorCode, errorMessage: errorDescription));
						}
					});
				});
			};

			XsollaLogin.Instance.LinkSocialProvider(socialProvider,urlCallback,onError);
		}

		private List<LinkedSocialNetwork> _networksCache;
		private DateTime _networksCacheTime;
		private bool _networksCacheInProgress;

		public void PurgeSocialProvidersCache() => _networksCache = null;

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
		public void SignInConsoleAccount(string userId, string platform, Action<string> onSuccess, Action<Error> onError)
		{
			Action<string> successCallback = token =>
			{
				onSuccess?.Invoke(token);
				LoginEvent?.Invoke();
			};
			
			XsollaLogin.Instance.SignInConsoleAccount(userId, platform, successCallback, onError);
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
			Action successCallback = () =>
			{
				onSuccess?.Invoke();
				UpdateUserAttributesEvent?.Invoke();
			};
			
			XsollaLogin.Instance.UpdateUserAttributes(token, projectId, attributes, successCallback, onError);
		}

		public void RemoveUserAttributes(string token, string projectId, List<string> attributeKeys, Action onSuccess, Action<Error> onError)
		{
			Action successCallback = () =>
			{
				onSuccess?.Invoke();
				RemoveUserAttributesEvent?.Invoke();
			};
			
			XsollaLogin.Instance.RemoveUserAttributes(token, projectId, attributeKeys, successCallback, onError);
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

		#region Passwordless
		public void StartAuthByPhoneNumber(string phoneNumber, string linkUrl, bool sendLink, Action<string> onSuccess, Action<Error> onError = null)
		{
			XsollaLogin.Instance.StartAuthByPhoneNumber(phoneNumber, linkUrl, sendLink, onSuccess, onError);
		}

		public void CompleteAuthByPhoneNumber(string phoneNumber, string confirmationCode, string operationId, Action<string> onSuccess, Action<Error> onError = null)
		{
			Action<string> successCallback = token =>
			{
				onSuccess?.Invoke(token);
				LoginEvent?.Invoke();
			};
			
			XsollaLogin.Instance.CompleteAuthByPhoneNumber(phoneNumber, confirmationCode, operationId, successCallback, onError);
		}

		public void StartAuthByEmail(string email, string linkUrl, bool sendLink, Action<string> onSuccess, Action<Error> onError = null)
		{
			XsollaLogin.Instance.StartAuthByEmail(email, linkUrl, sendLink, onSuccess, onError);
		}

		public void CompleteAuthByEmail(string email, string confirmationCode, string operationId, Action<string> onSuccess, Action<Error> onError = null)
		{
			Action<string> successCallback = token =>
			{
				onSuccess?.Invoke(token);
				LoginEvent?.Invoke();
			};
			
			XsollaLogin.Instance.CompleteAuthByEmail(email, confirmationCode, operationId, successCallback, onError);
		}
		#endregion
	}
}
