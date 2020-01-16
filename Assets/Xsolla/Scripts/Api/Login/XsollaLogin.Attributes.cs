using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Xsolla.Core;

namespace Xsolla.Login
{
	public partial class XsollaLogin : MonoSingleton<XsollaLogin>
	{
		private const string URL_USER_GET_ATTRIBUTES = "https://login.xsolla.com/api/attributes/users/me/get";
		private const string URL_USER_UPDATE_ATTRIBUTES = "https://login.xsolla.com/api/attributes/users/me/update";

		public void GetUserAttributes(string token, string projectId, List<string> attributeKeys, string userId, [NotNull] Action<List<UserAttribute>> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var getAttributesRequestBody = new GetAttributesJson(attributeKeys, projectId, userId);
			
			var headers = new List<WebRequestHeader>();
			headers.Add(WebRequestHeader.AuthHeader(token));
			headers.Add(WebRequestHeader.ContentTypeHeader());
			
			WebRequestHelper.Instance.PostRequest(URL_USER_GET_ATTRIBUTES, getAttributesRequestBody, headers, onSuccess, onError);
		}

		public void UpdateUserAttributes(string projectId, Action onSuccess, Action<Error> onError)
		{
			// TODO
		}
		
		public void RemoveUserAttributes(string projectId, Action onSuccess, Action<Error> onError)
		{
			// TODO
		}
	}
}