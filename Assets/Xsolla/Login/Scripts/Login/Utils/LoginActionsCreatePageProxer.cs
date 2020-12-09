using System;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class LoginActionsCreatePageProxer : MonoBehaviour
	{
		[SerializeField] SimpleSocialButton[] SocialLoginButtons = default;
		[SerializeField] SimpleButton SteamLoginButton = default;

		private void Awake()
		{
			if (SocialLoginButtons != null)
			{
				foreach (var socialButton in SocialLoginButtons)
				{
					if (socialButton != null)
						socialButton.onClick += () => RequestSocialAuth(socialButton.SocialProvider);
				}
			}

			if (SteamLoginButton != null)
				SteamLoginButton.onClick += RequestSteamAuth;
		}

		private void RequestSocialAuth(SocialProvider provider)
		{
			ExecuteProxyRequest(LoginProxyActions.RunSocialAuthDelegate, provider);
		}

		private void RequestSteamAuth()
		{
			ExecuteProxyRequest(LoginProxyActions.RunSteamAuthDelegate, null);
		}

		private void ExecuteProxyRequest(Action<LoginPageEnterController, object> proxyRequest, object proxyArgument)
		{
			var proxyObject = new GameObject();
			var proxyScript = proxyObject.AddComponent<LoginProxyActionHolder>();

			proxyScript.ProxyAction = proxyRequest;
			proxyScript.ProxyActionArgument = proxyArgument;

			DemoController.Instance.SetState(MenuState.Authorization);
		}
	}
}
