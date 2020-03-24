using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper : MonoSingleton<WebRequestHelper>
	{
		public void AddOptionalHeadersTo(UnityWebRequest request)
		{
			if (Application.platform != RuntimePlatform.WebGLPlayer) {
				request.SetRequestHeader("X-ENGINE", "UNITY");
				request.SetRequestHeader("X-ENGINE-V", Application.unityVersion.ToUpper());
				request.SetRequestHeader("X-SDK", "STORE");
				request.SetRequestHeader("X-SDK-V", Constants.StoreSdkVersion);
			}
		}

		public void AddContentTypeHeaderTo(UnityWebRequest request)
		{
			WebRequestHeader contentHeader = WebRequestHeader.ContentTypeHeader();
			request.SetRequestHeader(contentHeader.Name, contentHeader.Value);
		}
	}
}

