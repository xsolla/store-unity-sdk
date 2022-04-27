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
		/// Gets a list of particular user’s attributes.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get User's Attributes from Client</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/attributes/get-user-attributes-from-client"/>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="publisherProjectId">Project ID from Publisher Account which you want to get attributes for. If you do not specify it, it returns attributes without the value of this parameter.</param>
		/// <param name="attributeType">Type of attributes to get.</param>
		/// <param name="keys">List of attributes� keys which you want to get. If you do not specify them, it returns all user�s attributes.</param>
		/// <param name="userId">User ID which attributes you want to get. Returns only attributes with the `public` value of the `permission` parameter. If you do not specify it or put your user ID there, it returns only your attributes with any value for the `permission` parameter.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="UpdateUserAttributes"/>
		/// <seealso cref="RemoveUserAttributes"/>
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
		/// Updates and creates particular user’s attributes.
		/// </summary>
		/// <remarks> Swagger method name:<c>Update User's Attributes from Client</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/attributes/update-users-attributes-from-client"/>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="publisherProjectId">Project ID from Publisher Account which you want to update the value of specified attributes for. If you do not specify it, it updates attributes that are general to all games only.</param>
		/// <param name="attributes">List of attributes of the specified game. To add attribute which does not exist, set this attribute to the `key` parameter. To update `value` of the attribute, specify its `key` parameter and set the new `value`. You can change several attributes at a time.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="GetUserAttributes"/>
		/// <seealso cref="RemoveUserAttributes"/>
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
		/// Removes particular user's attributes.
		/// </summary>
		/// <remarks> Swagger method name:<c>Update User's Attributes from Client</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/attributes/update-users-attributes-from-client"/>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="publisherProjectId">Project ID from Publisher Account which you want to update the value of specified attributes for. If you do not specify it, it updates attributes that are general to all games only.</param>
		/// <param name="removingKeys">List of attributes which you want to delete. If you specify the same attribute in `attributes` parameter, it will not be deleted.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="GetUserAttributes"/>
		/// <seealso cref="UpdateUserAttributes"/>
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
