using System;
using UnityEngine;
using UnityEngine.UI;

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
		ExecuteProxyRequest(LoginActionExamples.RunSocialAuthDelegate, provider);
	}

	private void RequestSteamAuth()
	{
		ExecuteProxyRequest(LoginActionExamples.RunSteamAuthDelegate, null);
	}

	private void ExecuteProxyRequest(Action<LoginPageEnterController, object> proxyRequest, object proxyArgument)
	{
		var proxyObject = new GameObject();
		var proxyScript = proxyObject.AddComponent<LoginCreateToAuthProxyRequestHolder>();

		proxyScript.ProxyRequest = proxyRequest;
		proxyScript.ProxyArgument = proxyArgument;

		DemoController.Instance.SetState(MenuState.Authorization);
	}
}
