using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Login;

public class OneSocialAuth : MonoBehaviour, ILoginAuthorization
{
	public Action<string> OnSuccess { get; set; }
	public Action OnFailed { get; set; }

	[SerializeField]
	private SocialProvider provider;
	[SerializeField]
	private bool invalidateUserOldJwt;
	
	private Button _authButton;

	private void Start()
	{
#if !UNITY_STANDALONE && !UNITY_EDITOR
		gameObject.SetActive(false);
#else
		_authButton = GetComponent<Button>();
	}

	public void Enable()
	{
		if (_authButton != null)
		{
			_authButton.onClick.AddListener(SocialNetworkAuth);
		}
	}
	
	private void SocialNetworkAuth()
	{
		if(HotkeyCoroutine.IsLocked()) return;
		HotkeyCoroutine.Lock();

		string url = XsollaLogin.Instance.GetSocialNetworkAuthUrl(provider, invalidateUserOldJwt);
		Debug.Log($"Social url: {url}");
		BrowserHelper.Instance.Open(url, true);
		var singlePageBrowser = BrowserHelper.Instance.GetLastBrowser();
		singlePageBrowser.BrowserClosedEvent += _ => BrowserCloseHandler();
		BrowserHelper.Instance.GetLastBrowser().GetComponent<XsollaBrowser>().Navigate.UrlChangedEvent +=
			(browser, newUrl) =>
			{
				if (IsRedirectWithToken(newUrl, out var token))
				{
					Debug.Log($"We take{Environment.NewLine}from URL:{newUrl}{Environment.NewLine}token = {token}");
					StartCoroutine(SuccessAuthCoroutine(token));
				}
			};
	}
	
	private void BrowserCloseHandler()
	{
		HotkeyCoroutine.Unlock();
	}

	private IEnumerator SuccessAuthCoroutine(string token)
	{
		yield return new WaitForEndOfFrame();
		Destroy(BrowserHelper.Instance.gameObject);
		BrowserCloseHandler();
		OnSuccess?.Invoke(token);
	}
	
	private static bool IsRedirectWithToken(string newUrl, out string token)
	{
		var regex = new Regex(@"[&?]token=\S*");
		token = regex.Match(newUrl).Value.Replace("?token=", string.Empty).Replace("&token=", string.Empty);
		return !string.IsNullOrEmpty(token);
#endif
	}
}
