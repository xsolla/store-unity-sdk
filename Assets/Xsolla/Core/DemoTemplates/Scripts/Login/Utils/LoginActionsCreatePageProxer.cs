using System;
using UnityEngine;
using UnityEngine.UI;

public class LoginActionsCreatePageProxer : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] SimpleSocialButton[] SocialLoginButtons;
	[SerializeField] SimpleButton SteamLoginButton;
#pragma warning restore 0649

	public Action<Action<LoginPageEnterController, object>, object> OnActionRequest;

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
		OnActionRequest?.Invoke(LoginActionExamples.RunSocialAuthDelegate, provider);
	}

	private void RequestSteamAuth()
	{
		OnActionRequest?.Invoke(LoginActionExamples.RunSteamAuthDelegate, null);
	}
}
