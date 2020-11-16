using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Xsolla.Core;

namespace Xsolla.Login
{
	public partial class XsollaLogin : MonoSingleton<XsollaLogin>
	{
		private const string URL_USER_GET_ATTRIBUTES = "https://login.xsolla.com/api/attributes/users/me/get";
		private const string URL_USER_GET_READONLY_ATTRIBUTES = "https://login.xsolla.com/api/attributes/users/me/get_read_only";
		private const string URL_USER_UPDATE_ATTRIBUTES = "https://login.xsolla.com/api/attributes/users/me/update";

		/// <summary>
		/// Returns user attributes.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get User's Attributes from Client</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/attributes/get-user-attributes-from-client"/>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="attributeKeys">Attributes names list filter</param>
		/// <param name="userId">Login user ID. Can be null, because this info exist in the token.</param>
		/// <param name="attributeType">User attribute type to get</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="UpdateUserAttributes"/>
		/// <seealso cref="RemoveUserAttributes"/>
		public void GetUserAttributes(string token, string projectId, UserAttributeType attributeType, [CanBeNull] List<string> attributeKeys, [CanBeNull] string userId, [NotNull] Action<List<UserAttribute>> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var getAttributesRequestBody = new GetAttributesJson(attributeKeys, projectId, userId);
			
			var headers = new List<WebRequestHeader>();
			headers.Add(WebRequestHeader.AuthHeader(token));
			headers.Add(WebRequestHeader.ContentTypeHeader());
			
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

			WebRequestHelper.Instance.PostRequest(url, getAttributesRequestBody, headers, onSuccess, onError);
		}

		/// <summary>
		/// Updates user attributes' values.
		/// </summary>
		/// <remarks> Swagger method name:<c>Update User's Attributes from Client</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/attributes/update-users-attributes-from-client"/>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="attributes">Attributes list.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="GetUserAttributes"/>
		/// <seealso cref="RemoveUserAttributes"/>
		public void UpdateUserAttributes(string token, string projectId, List<UserAttribute> attributes, Action onSuccess, Action<Error> onError)
		{
			var modifyAttributesRequestBody = new ModifyAttributesJson(attributes, projectId, null);
			
			var headers = new List<WebRequestHeader>();
			headers.Add(WebRequestHeader.AuthHeader(token));
			headers.Add(WebRequestHeader.ContentTypeHeader());
			
			WebRequestHelper.Instance.PostRequest(URL_USER_UPDATE_ATTRIBUTES, modifyAttributesRequestBody, headers, onSuccess, onError);
		}

		/// <summary>
		/// Removes user attributes.
		/// </summary>
		/// <remarks> Swagger method name:<c>Update User's Attributes from Client</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/login-api/attributes/update-users-attributes-from-client"/>
		/// <param name="token">JWT from Xsolla Login.</param>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="attributeKeys">Attributes names list.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="GetUserAttributes"/>
		/// <seealso cref="UpdateUserAttributes"/>
		public void RemoveUserAttributes(string token, string projectId, List<string> attributeKeys, Action onSuccess, Action<Error> onError)
		{
			var removeAttributesRequestBody = new ModifyAttributesJson(null, projectId, attributeKeys);
			
			var headers = new List<WebRequestHeader>();
			headers.Add(WebRequestHeader.AuthHeader(token));
			headers.Add(WebRequestHeader.ContentTypeHeader());
			
			WebRequestHelper.Instance.PostRequest(URL_USER_UPDATE_ATTRIBUTES, removeAttributesRequestBody, headers, onSuccess, onError);
		}
	}
}
