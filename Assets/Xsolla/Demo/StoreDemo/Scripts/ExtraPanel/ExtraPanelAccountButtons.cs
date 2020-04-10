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
	const string URL_MASTER_ACCOUNT = "https://livedemo.xsolla.com/igs-demo/#/";

	[SerializeField] private GameObject signOutButton;

	[SerializeField] private GameObject accountLinkingButton;

	[SerializeField] private GameObject requestCodeButton;

	public void Init()
	{
		signOutButton.SetActive(true);
		accountLinkingButton.SetActive(!XsollaStore.Instance.Token.IsMasterAccount());
		requestCodeButton.SetActive(XsollaStore.Instance.Token.IsMasterAccount());

		var btnComponent = signOutButton.GetComponent<SimpleTextButton>();
		btnComponent.onClick = () => {
			LauncherArguments.Instance.InvalidateTokenArguments();
			SceneManager.LoadScene("Login");
		};
		
		requestCodeButton.SetActive(false);
		// TODO: coming soon
		// var requestCodeButtonComponent = requestCodeButton.GetComponent<SimpleTextButton>();
		// requestCodeButtonComponent.onClick = () => {
		// 	XsollaLogin.Instance.RequestLinkingCode(
		// 		(LinkingCode code) => PopupFactory.Instance.CreateSuccess().SetMessage("YOUR CODE: " + code.code),
		// 		ShowError);
		// };
		
		accountLinkingButton.SetActive(false);
		// TODO: coming soon
		// var accountLinkingButtonComponent = accountLinkingButton.GetComponent<SimpleTextButton>();
		// accountLinkingButtonComponent.onClick = () => {
		// 	ShowCodeConfirmation((string code) => {
		// 		XsollaLogin.Instance.LinkConsoleAccount(
		// 			XsollaSettings.UsernameFromConsolePlatform,
		// 			XsollaSettings.Platform.GetString(),
		// 			code,
		// 			LinkingAccountHandler,
		// 			ShowError);
		// 	});
		// 	OpenUrlEvent?.Invoke(URL_MASTER_ACCOUNT);
		// };
	}

	private void LinkingAccountHandler()
	{
		PopupFactory.Instance.CreateSuccess();
		XsollaLogin.Instance.SignInConsoleAccount(
			XsollaSettings.UsernameFromConsolePlatform,
			XsollaSettings.Platform.GetString(),
			ReloginCallback,
			ShowError
		);
	}

	private void ReloginCallback(string newToken)
	{
		XsollaLogin.Instance.Token = XsollaStore.Instance.Token = newToken;
		LinkingAccountComplete?.Invoke();
		accountLinkingButton.SetActive(false);
		requestCodeButton.SetActive(true);
	}

	private static void ShowError(Error error)
	{
		PopupFactory.Instance.CreateError().SetMessage(error.ToString());
	}

	private void ShowCodeConfirmation(Action<string> callback)
	{
		PopupFactory.Instance.CreateCodeConfirmation().SetConfirmCallback(callback);
	}
}
