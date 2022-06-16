using System;
using System.Collections.Generic;
using Xsolla.Auth;
using Xsolla.Core;
using Xsolla.UserAccount;

namespace Xsolla.Login
{
	public partial class XsollaLogin : MonoSingleton<XsollaLogin>
	{
		[Obsolete("Use XsollaAuth instead")]
		public void GetUserInfo(string token, Action<UserInfo> onSuccess, Action<Error> onError = null) => XsollaAuth.Instance.GetUserInfo(token, onSuccess, onError);

		[Obsolete("Use XsollaUserAccount instead")]
		public void UpdateUserInfo(string token, UserInfoUpdate info, Action<UserInfo> onSuccess, Action<Error> onError = null)
			=> XsollaUserAccount.Instance.UpdateUserInfo(token, info, onSuccess, onError);

		[Obsolete("Use XsollaAuth instead")]
		public void Registration(string username, string password, string email, string redirectUri = null, string state = null, string payload = null, bool? acceptConsent = null, bool? promoEmailAgreement = null, List<string> fields = null, Action<int> onSuccess = null, Action<Error> onError = null)
			=> XsollaAuth.Instance.Register(username, password, email, redirectUri, state, payload, acceptConsent, promoEmailAgreement, fields, onSuccess:onSuccess, onError:onError);

		[Obsolete("Use XsollaAuth instead")]
		public void Registration(string username, string password, string email, string redirectUri = null, string oauthState = null, string payload = null, bool? acceptConsent = null, bool? promoEmailAgreement = null, List<string> fields = null, Action onSuccess = null, Action<Error> onError = null)
			=> XsollaAuth.Instance.Register(username, password, email, redirectUri, oauthState, payload, acceptConsent, promoEmailAgreement, fields, onSuccess:onSuccess, onError:onError);

		[Obsolete("Use XsollaAuth instead")]
		public void Registration(string username, string password, string email, string redirectUri = null, string oauthState = null, string payload = null, bool? acceptConsent = null, bool? promoEmailAgreement = null, List<string> fields = null, Action<LoginUrlResponse> onSuccess = null, Action<Error> onError = null)
			=> XsollaAuth.Instance.Register(username, password, email, redirectUri, oauthState, payload, acceptConsent, promoEmailAgreement, fields, onSuccess:onSuccess, onError:onError);

		[Obsolete("Use XsollaAuth instead")]
		public void SignIn(string username, string password, bool rememberMe, string redirectUri = null, string payload = null, Action<string> onSuccess = null, Action<Error> onError = null)
			=> XsollaAuth.Instance.SignIn(username, password, rememberMe, redirectUri, payload, onSuccess, onError);

		[Obsolete("Use XsollaAuth instead")]
		public void StartAuthByEmail(string email, string linkUrl, bool? sendLink, Action<string> onSuccess, Action<Error> onError = null)
			=> XsollaAuth.Instance.StartAuthByEmail(email, linkUrl, sendLink, onSuccess, onError);

		[Obsolete("Use XsollaAuth instead")]
		public void CompleteAuthByEmail(string email, string confirmationCode, string operationId, Action<string> onSuccess, Action<Error> onError = null)
			=> XsollaAuth.Instance.CompleteAuthByEmail(email, confirmationCode, operationId, onSuccess, onError);

		[Obsolete("Use XsollaAuth instead")]
		public void StartAuthByPhoneNumber(string phoneNumber, string linkUrl, bool sendLink, Action<string> onSuccess, Action<Error> onError = null)
			=> XsollaAuth.Instance.StartAuthByPhoneNumber(phoneNumber, linkUrl, sendLink, onSuccess, onError);

		[Obsolete("Use XsollaAuth instead")]
		public void CompleteAuthByPhoneNumber(string phoneNumber, string confirmationCode, string operationId, Action<string> onSuccess, Action<Error> onError = null)
			=> XsollaAuth.Instance.CompleteAuthByPhoneNumber(phoneNumber, confirmationCode, operationId, onSuccess, onError);

		[Obsolete("Use XsollaAuth instead")]
		public void ResetPassword(string email, string redirectUri = null, Action onSuccess = null, Action<Error> onError = null)
			=> XsollaAuth.Instance.ResetPassword(email, redirectUri, onSuccess:onSuccess, onError:onError);

		[Obsolete("Use XsollaAuth instead")]
		public void ResendConfirmationLink(string username, string redirectUri = null, string state = null, string payload = null, Action onSuccess = null, Action<Error> onError = null)
			=> XsollaAuth.Instance.ResendConfirmationLink(username, redirectUri, state, payload, onSuccess:onSuccess, onError:onError);

		[Obsolete("Use XsollaUserAccount instead")]
		public void SearchUsers(string token, string nickname, uint offset, uint limit, Action<FoundUsers> onSuccess, Action<Error> onError = null)
			=> XsollaUserAccount.Instance.SearchUsers(token, nickname, offset, limit, onSuccess, onError);

		[Obsolete("Use XsollaUserAccount instead")]
		public void GetPublicInfo(string token, string userId, Action<UserPublicInfo> onSuccess, Action<Error> onError = null)
			=> XsollaUserAccount.Instance.GetPublicInfo(token, userId, onSuccess, onError);

		[Obsolete("Use XsollaUserAccount instead")]
		public void GetUserPhoneNumber(string token, Action<string> onSuccess, Action<Error> onError)
			=> XsollaUserAccount.Instance.GetUserPhoneNumber(token, onSuccess, onError);

		[Obsolete("Use XsollaUserAccount instead")]
		public void ChangeUserPhoneNumber(string token, string phoneNumber, Action onSuccess, Action<Error> onError)
			=> XsollaUserAccount.Instance.UpdateUserPhoneNumber(token, phoneNumber, onSuccess, onError);

		[Obsolete("Use XsollaUserAccount instead")]
		public void DeleteUserPhoneNumber(string token, string phoneNumber, Action onSuccess, Action<Error> onError)
			=> XsollaUserAccount.Instance.DeleteUserPhoneNumber(token, phoneNumber, onSuccess, onError);

		[Obsolete("Use XsollaUserAccount instead")]
		public void UploadUserPicture(string token, byte[] pictureData, string boundary, Action<string> onSuccess, Action<Error> onError)
			=> XsollaUserAccount.Instance.UploadUserPicture(token, pictureData, boundary, onSuccess, onError);

		[Obsolete("Use XsollaUserAccount instead")]
		public void DeleteUserPicture(string token, Action onSuccess, Action<Error> onError)
			=> XsollaUserAccount.Instance.DeleteUserPicture(token, onSuccess, onError);

		[Obsolete("Use XsollaUserAccount instead")]
		public void CheckUserAge(string dateOfBirth, Action<UserCheckAgeResult> onSuccess, Action<Error> onError)
			=> XsollaUserAccount.Instance.CheckUserAge(dateOfBirth, onSuccess, onError);

		[Obsolete("Use XsollaUserAccount instead")]
		public void GetUserEmail(string token, Action<string> onSuccess, Action<Error> onError)
			=> XsollaUserAccount.Instance.GetUserEmail(token, onSuccess, onError);

		[Obsolete("Use XsollaAuth instead")]
		public void AuthWithSocialNetworkAccessToken(string accessToken, string accessTokenSecret, string openId, string providerName, string payload, string state = null, Action<string> onSuccess = null, Action<Error> onError = null)
			=> XsollaAuth.Instance.AuthWithSocialNetworkAccessToken(accessToken, accessTokenSecret, openId, providerName, payload, state, onSuccess, onError);

		[Obsolete("Use XsollaAuth instead")]
		public void OAuthLogout(string token, OAuthLogoutType sessions, Action onSuccess, Action<Error> onError = null)
			=> XsollaAuth.Instance.OAuthLogout(token, sessions, onSuccess, onError);
	}
}
