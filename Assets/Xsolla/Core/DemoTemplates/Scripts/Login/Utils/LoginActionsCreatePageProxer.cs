using System;
using UnityEngine;

public class LoginActionsCreatePageProxer : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] SimpleSocialButton[] SocialLoginButtons;
	[SerializeField] SimpleButton SteamLoginButton;
#pragma warning restore 0649

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
