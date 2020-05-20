using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Xsolla.Core;
using Xsolla.Core.Popup;
using Xsolla.Login;
using Xsolla.Store;

[AddComponentMenu("Scripts/Xsolla.Store/Extra/ExtraPanelAccountButtons")]
public class ExtraPanelAccountButtons : MonoBehaviour
{
	public event Action<string> OpenUrlEvent;
	public event Action LinkingAccountComplete;
	private const string URL_MASTER_ACCOUNT = "https://livedemo.xsolla.com/igs-demo/#/";

	[SerializeField] private GameObject signOutButton;
	[SerializeField] private GameObject accountLinkingButton;
	[SerializeField] private GameObject requestCodeButton;

	public void Init()
	{
		signOutButton.SetActive(true);
		var btnComponent = signOutButton.GetComponent<SimpleTextButton>();
		btnComponent.onClick = () => {
			LauncherArguments.Instance.InvalidateTokenArguments();
			XsollaLogin.Instance.DeleteToken(Constants.LAST_SUCCESS_AUTH_TOKEN);
			SceneManager.LoadScene("Login");
		};

		if (XsollaStore.Instance.Token.IsMasterAccount())
			RequestCodeEnable();
		else
			LinkingButtonEnable();
	}

	private void RequestCodeEnable()
	{
		accountLinkingButton.SetActive(false);
		requestCodeButton.SetActive(true);
		var requestCodeButtonComponent = requestCodeButton.GetComponent<SimpleTextButton>();
		requestCodeButtonComponent.onClick = () => {
			XsollaLogin.Instance.RequestLinkingCode(
				code => StoreDemoPopup.ShowSuccess($"YOUR CODE: {code.code}"),
				StoreDemoPopup.ShowError);
		};
	}

	private void LinkingButtonEnable()
	{
		requestCodeButton.SetActive(false);
		accountLinkingButton.SetActive(true);
		var accountLinkingButtonComponent = accountLinkingButton.GetComponent<SimpleTextButton>();
		accountLinkingButtonComponent.onClick = () => {
			ShowCodeConfirmation(code => {
				XsollaLogin.Instance.LinkConsoleAccount(
					XsollaSettings.UsernameFromConsolePlatform,
					XsollaSettings.Platform.GetString(),
					code,
					LinkingAccountHandler,
					StoreDemoPopup.ShowError);
			});
			OpenUrlEvent?.Invoke(URL_MASTER_ACCOUNT);
		};
	}

	private void LinkingAccountHandler()
	{
		PopupFactory.Instance.CreateSuccess();
		XsollaLogin.Instance.SignInConsoleAccount(
			XsollaSettings.UsernameFromConsolePlatform,
			XsollaSettings.Platform.GetString(),
			ReloginCallback,
			StoreDemoPopup.ShowError
		);
	}

	private void ReloginCallback(string newToken)
	{
		XsollaLogin.Instance.Token = XsollaStore.Instance.Token = newToken;
		LinkingAccountComplete?.Invoke();
		if (XsollaStore.Instance.Token.IsMasterAccount())
			RequestCodeEnable();
		else
			LinkingButtonEnable();
	}

	private void ShowCodeConfirmation(Action<string> callback)
	{
		PopupFactory.Instance.CreateCodeConfirmation().SetConfirmCallback(callback);
	}
}
