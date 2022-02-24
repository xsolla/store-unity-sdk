using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core;
using Xsolla.UserAccount;

namespace Xsolla.Demo
{
	public class SdkUserAccountLogic : MonoSingleton<SdkUserAccountLogic>
	{
		private const int NETWORKS_CACHE_TIMEOUT = 5;

		public event Action UpdateUserAttributesEvent;
		public event Action RemoveUserAttributesEvent;

		#region User
		public void GetPublicInfo(string token, string user, Action<UserPublicInfo> onSuccess, Action<Error> onError = null)
		{
			XsollaUserAccount.Instance.GetPublicInfo(token, user, onSuccess, onError = null);
		}

		public void ChangeUserPhoneNumber(string token, string phoneNumber, Action onSuccess, Action<Error> onError)
		{
			XsollaUserAccount.Instance.ChangeUserPhoneNumber(token, phoneNumber, onSuccess, onError);
		}

		public void DeleteUserPhoneNumber(string token, string phoneNumber, Action onSuccess, Action<Error> onError)
		{
			XsollaUserAccount.Instance.DeleteUserPhoneNumber(token, phoneNumber, onSuccess, onError);
		}

		public void SearchUsersByNickname(string nickname, Action<List<FriendModel>> onSuccess = null, Action<Error> onError = null)
		{
			nickname = Uri.EscapeDataString(nickname);

			XsollaUserAccount.Instance.SearchUsers(Token.Instance, nickname, 0, 20,
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

			XsollaUserAccount.Instance.LinkSocialProvider(socialProvider, urlCallback, onError);
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
				XsollaUserAccount.Instance.GetLinkedSocialProviders(networks =>
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
		public void RequestLinkingCode(Action<LinkingCode> onSuccess, Action<Error> onError)
		{
			XsollaUserAccount.Instance.RequestLinkingCode(onSuccess, onError);
		}

		public void LinkConsoleAccount(string userId, string platform, string confirmationCode, Action onSuccess, Action<Error> onError)
		{
			XsollaUserAccount.Instance.LinkConsoleAccount(userId, platform, confirmationCode, onSuccess, onError);
		}
		#endregion

		#region Attributes
		public void GetUserAttributes(string token, string projectId, UserAttributeType attributeType,
			List<string> attributeKeys, string userId, Action<List<UserAttribute>> onSuccess, Action<Error> onError)
		{
			XsollaUserAccount.Instance.GetUserAttributes(token, projectId, attributeType, attributeKeys, userId, onSuccess, onError);
		}

		public void UpdateUserAttributes(string token, string projectId, List<UserAttribute> attributes, Action onSuccess, Action<Error> onError)
		{
			Action successCallback = () =>
			{
				onSuccess?.Invoke();
				UpdateUserAttributesEvent?.Invoke();
			};

			XsollaUserAccount.Instance.UpdateUserAttributes(token, projectId, attributes, successCallback, onError);
		}

		public void RemoveUserAttributes(string token, string projectId, List<string> attributeKeys, Action onSuccess, Action<Error> onError)
		{
			Action successCallback = () =>
			{
				onSuccess?.Invoke();
				RemoveUserAttributesEvent?.Invoke();
			};

			XsollaUserAccount.Instance.RemoveUserAttributes(token, projectId, attributeKeys, successCallback, onError);
		}
		#endregion

		#region Picture
		public void UploadUserPicture(string token, byte[] pictureData, string boundary, Action<string> onSuccess, Action<Error> onError)
		{
			XsollaUserAccount.Instance.UploadUserPicture(token, pictureData, boundary, onSuccess, onError);
		}

		public void DeleteUserPicture(string token, Action onSuccess, Action<Error> onError)
		{
			XsollaUserAccount.Instance.DeleteUserPicture(token, onSuccess, onError);
		}
		#endregion
	}
}
