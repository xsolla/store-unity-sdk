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
	const string URL_MASTER_ACCOUNT = "https://livedemo.xsolla.com/sdk/unity/webgl/";

	[SerializeField]
	GameObject signOutButton;

	[SerializeField]
	GameObject accountLinkingButton;

	[SerializeField]
	GameObject requestCodeButton;

	public void Init()
	{
		signOutButton.SetActive(true);
		accountLinkingButton.SetActive(XsollaSettings.IsShadow);
		requestCodeButton.SetActive(!XsollaSettings.IsShadow);

		var btnComponent = signOutButton.GetComponent<SimpleTextButton>();
		btnComponent.onClick = () => {
			LauncherArguments.Instance.InvalidateTokenArguments();
			SceneManager.LoadScene("Login");
		};

		var requestCodeButtonComponent = requestCodeButton.GetComponent<SimpleTextButton>();
		requestCodeButtonComponent.onClick = () => {
			XsollaLogin.Instance.RequestLinkingCode(
				(LinkingCode code) => PopupFactory.Instance.CreateSuccess().SetMessage("YOUR CODE: " + code.code),
				ShowError);
		};

		var accountLinkingButtonComponent = accountLinkingButton.GetComponent<SimpleTextButton>();
		accountLinkingButtonComponent.onClick = () => {
			ShowCodeConfirmation((string code) => {
				XsollaLogin.Instance.LinkAccount(XsollaLogin.Instance.ShadowAccountUserID, code,
				LinkingAccountHandler, ShowError);
			});
			OpenUrlEvent?.Invoke(URL_MASTER_ACCOUNT);
		};
	}

	private void LinkingAccountHandler()
	{
		PopupFactory.Instance.CreateSuccess();
		XsollaLogin.Instance.SignInShadowAccount(
			XsollaLogin.Instance.ShadowAccountUserID,
			ReloginWithShadowAccountCallback,
			ShowError
		);
	}

	private void ReloginWithShadowAccountCallback(string newToken)
	{
		XsollaLogin.Instance.Token = XsollaStore.Instance.Token = newToken;
		LinkingAccountComplete?.Invoke();
		accountLinkingButton.SetActive(false);
		requestCodeButton.SetActive(true);
	}

	private void ShowError(Error error)
	{
		PopupFactory.Instance.CreateError().SetMessage(error.ToString());
	}

	private void ShowCodeConfirmation(Action<string> callback)
	{
		PopupFactory.Instance.CreateCodeConfirmation().SetConfirmCallback(callback);
	}
}
