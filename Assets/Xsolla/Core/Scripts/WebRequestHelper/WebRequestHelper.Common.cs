using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper : MonoSingleton<WebRequestHelper>
	{
		public void AddOptionalHeadersTo(UnityWebRequest request)
		{
			if (Application.platform != RuntimePlatform.WebGLPlayer)
				GetAdditionalHeaders().ForEach(h => request.SetRequestHeader(h.Name, h.Value));
		}

		public List<WebRequestHeader> GetAdditionalHeaders()
		{
			return new List<WebRequestHeader>
			{
				new WebRequestHeader("X-ENGINE", "UNITY"),
				new WebRequestHeader("X-ENGINE-V", Application.unityVersion.ToUpper()),
				new WebRequestHeader("X-SDK", "STORE"),
				new WebRequestHeader("X-SDK-V", Constants.StoreSdkVersion)
			};
		}

		public void AddContentTypeHeaderTo(UnityWebRequest request)
		{
			var contentHeader = GetContentTypeHeader();
			request.SetRequestHeader(contentHeader.Name, contentHeader.Value);
		}

		public WebRequestHeader GetContentTypeHeader()
		{
			return WebRequestHeader.ContentTypeHeader();
		}
	}
}

