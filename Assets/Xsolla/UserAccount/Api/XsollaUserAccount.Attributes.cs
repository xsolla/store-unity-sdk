using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Xsolla.Core;

namespace Xsolla.UserAccount
{
	public partial class XsollaUserAccount : MonoSingleton<XsollaUserAccount>
	{
		private const string URL_USER_GET_ATTRIBUTES = "https://login.xsolla.com/api/attributes/users/me/get";
		private const string URL_USER_GET_READONLY_ATTRIBUTES = "https://login.xsolla.com/api/attributes/users/me/get_read_only";
		private const string URL_USER_UPDATE_ATTRIBUTES = "https://login.xsolla.com/api/attributes/users/me/update";

		/// <summary>
		/// Returns a list of particular user’s attributes with their values and descriptions. Returns only user-editable attributes.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/user-attributes/).</remarks>
		/// <param name="token">User authorization token.</param>
		/// <param name="publisherProjectId">Project ID from Publisher Account which you want to get attributes for. If you do not specify it, it returns attributes that were created without this parameter.</param>
		/// <param name="attributeType">Type of attributes to get. Can be `Readonly` or `Custom`.</param>
		/// <param name="keys">List of attributes’ keys which you want to get. If not specified, the method returns all user’s attributes.</param>
		/// <param name="userId">Identifier of a user whose public attributes should be requested. If not specified, the method returns attrubutes for the current user.</param>
		/// <param name="onSuccess">Called after user attributes were successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <seealso href="UpdateUserAttributes"/>
		/// <seealso href="RemoveUserAttributes"/>
		public void GetUserAttributes(string token, string publisherProjectId, UserAttributeType attributeType, [CanBeNull] List<string> keys, [CanBeNull] string userId, [NotNull] Action<List<UserAttribute>> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var getAttributesRequestBody = new GetAttributesJson(keys, publisherProjectId, userId);
			var headers = new List<WebRequestHeader> { WebRequestHeader.AuthHeader(token), WebRequestHeader.ContentTypeHeader() };

			string url = default(string);

			switch (attributeType)
			{
				case UserAttributeType.CUSTOM:
					url = URL_USER_GET_ATTRIBUTES;
					break;
				case UserAttributeType.READONLY:
					url = URL_USER_GET_READONLY_ATTRIBUTES;
					break;
			}

			WebRequestHelper.Instance.PostRequest(SdkType.Login, url, getAttributesRequestBody, headers, onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => GetUserAttributes(Token.Instance, publisherProjectId, attributeType, keys, userId, onSuccess, onError)));
		}

		public void GetUserAttributes(string publisherProjectId, UserAttributeType attributeType, [CanBeNull] List<string> keys, [CanBeNull] string userId, [NotNull] Action<List<UserAttribute>> onSuccess, [CanBeNull] Action<Error> onError)
		{
			GetUserAttributes(Token.Instance, publisherProjectId, attributeType, keys, userId, onSuccess, onError);
		}

		/// <summary>
		/// Updates the values of user attributes with the specified IDs. The method can be used to create or remove attributes. Changes are made on the user data storage side (server side).
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/user-attributes/).</remarks>
		/// <param name="token">User authorization token.</param>
		/// <param name="publisherProjectId">Project ID from Publisher Account which you want to update the value of specified attributes for. If you do not specify it, it updates attributes that are general to all games only.</param>
		/// <param name="attributes">List of attributes of the specified game. To add attribute which does not exist, set this attribute to the `key` parameter. To update `value` of the attribute, specify its `key` parameter and set the new `value`. You can change several attributes at a time.</param>
		/// <param name="onSuccess">Called after successful user attributes modification on the server side.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <seealso href="GetUserAttributes"/>
		/// <seealso href="RemoveUserAttributes"/>
		public void UpdateUserAttributes(string token, string publisherProjectId, List<UserAttribute> attributes, Action onSuccess, Action<Error> onError)
		{
			var modifyAttributesRequestBody = new ModifyAttributesJson(attributes, publisherProjectId, null);
			var headers = new List<WebRequestHeader>() { WebRequestHeader.AuthHeader(token), WebRequestHeader.ContentTypeHeader() };

			WebRequestHelper.Instance.PostRequest(SdkType.Login, URL_USER_UPDATE_ATTRIBUTES, modifyAttributesRequestBody, headers, onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => UpdateUserAttributes(Token.Instance, publisherProjectId, attributes, onSuccess, onError)));
		}

		public void UpdateUserAttributes(string publisherProjectId, List<UserAttribute> attributes, Action onSuccess, Action<Error> onError)
		{
			UpdateUserAttributes(Token.Instance, publisherProjectId, attributes, onSuccess, onError);
		}

		/// <summary>
		/// Removes user attributes with the specified IDs. Changes are made on the user data storage side (server side).
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/user-attributes/).</remarks>
		/// <param name="token">User authorization token.</param>
		/// <param name="publisherProjectId">Project ID from Publisher Account which you want to get attributes for. If you do not specify it, it returns attributes that were created without this parameter.</param>
		/// <param name="removingKeys">List of attribute keys for removal.</param>
		/// <param name="onSuccess">Called after successful user attributes removal on the server side.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <seealso href="GetUserAttributes"/>
		/// <seealso href="UpdateUserAttributes"/>
		public void RemoveUserAttributes(string token, string publisherProjectId, List<string> removingKeys, Action onSuccess, Action<Error> onError)
		{
			var removeAttributesRequestBody = new ModifyAttributesJson(null, publisherProjectId, removingKeys);
			var headers = new List<WebRequestHeader>() { WebRequestHeader.AuthHeader(token), WebRequestHeader.ContentTypeHeader() };

			WebRequestHelper.Instance.PostRequest(SdkType.Login, URL_USER_UPDATE_ATTRIBUTES, removeAttributesRequestBody, headers, onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => RemoveUserAttributes(Token.Instance, publisherProjectId, removingKeys, onSuccess, onError)));
		}

		public void RemoveUserAttributes(string publisherProjectId, List<string> removingKeys, Action onSuccess, Action<Error> onError)
		{
			RemoveUserAttributes(Token.Instance, publisherProjectId, removingKeys, onSuccess, onError);
		}
	}
}
