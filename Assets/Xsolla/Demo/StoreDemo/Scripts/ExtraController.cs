using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Xsolla.Core;
using Xsolla.Login;
using Xsolla.Store;

public class ExtraController : MonoBehaviour
{
	[SerializeField]
	GameObject signOutButton;

	[SerializeField]
	GameObject attributesPanel;
	
	[SerializeField]
	GameObject registrationButton;
	
	[SerializeField]
	GameObject instructionButton;
	
	[SerializeField]
	GameObject testCardsButton;

	[SerializeField]
	GameObject accountLinkingButton;

	[SerializeField]
	GameObject requestCodeButton;

	AttributesSidePanel attributesSidePanel;
	StoreController store;

	const string URL_REGISTRATION = "https://publisher.xsolla.com/signup?store_type=sdk";
	const string URL_INSTRUCTION = "https://developers.xsolla.com/sdk/game-engines/unity/";
	const string URL_TEST_CARDS = "https://developers.xsolla.com/api/v1/pay-station/#api_payment_ui_test_cards";
	const string URL_MASTER_ACCOUNT = "https://livedemo.xsolla.com/sdk/unity/webgl/";

	private void Awake()
	{
		attributesSidePanel = attributesPanel.GetComponent<AttributesSidePanel>();
	}

	public void Init(StoreController storeController)
	{
		store = storeController;
		signOutButton.SetActive(true);
		registrationButton.SetActive(true);
		instructionButton.SetActive(true);
		testCardsButton.SetActive(true);
		accountLinkingButton.SetActive(XsollaSettings.IsShadow);
		requestCodeButton.SetActive(!XsollaSettings.IsShadow);

		var btnComponent = signOutButton.GetComponent<SimpleTextButton>();
		btnComponent.onClick = () => {
			LauncherArguments.Instance.InvalidateTokenArguments();
			SceneManager.LoadScene("Login");
		};

		var registrationBtnComponent = registrationButton.GetComponent<SimpleTextButton>();
		registrationBtnComponent.onClick = () => { OpenURL(URL_REGISTRATION); };

		var instructionBtnComponent = instructionButton.GetComponent<SimpleTextButton>();
		instructionBtnComponent.onClick = () => { OpenURL(URL_INSTRUCTION); };

		var testCardsBtnComponent = testCardsButton.GetComponent<SimpleTextButton>();
		testCardsBtnComponent.onClick = () => { OpenURL(URL_TEST_CARDS); };

		var requestCodeButtonComponent = requestCodeButton.GetComponent<SimpleTextButton>();
		requestCodeButtonComponent.onClick = () => {
			XsollaLogin.Instance.RequestLinkingCode(
				(LinkingCode code) => storeController.ShowSuccess("YOUR CODE: " + code.code),
				(Error error) => storeController.ShowError(error)
			);
		};

		var accountLinkingButtonComponent = accountLinkingButton.GetComponent<SimpleTextButton>();
		accountLinkingButtonComponent.onClick = () => {
			store.ShowConfirmCode((string code) => {
				XsollaLogin.Instance.LinkAccount(XsollaLogin.Instance.ShadowAccountUserID, code,
				() => {
					storeController.ShowSuccess();
					XsollaLogin.Instance.SignInShadowAccount(
						XsollaLogin.Instance.ShadowAccountUserID,
						(string token) => {
							XsollaLogin.Instance.Token = XsollaStore.Instance.Token = token;
							storeController.RefreshVirtualCurrencyBalance();
							storeController.RefreshAttributes();
							storeController.RefreshInventory(() => {
								accountLinkingButton.SetActive(false);
								requestCodeButton.SetActive(true);
							});
						},
						(Error error) => storeController.ShowError(error)
					);
				},
				(Error error) => storeController.ShowError(error)
			);
			});
			OpenURL(URL_MASTER_ACCOUNT);
		};
	}

	public void RefreshAttributesPanel()
	{
		attributesSidePanel.Refresh();
	}

	public void ShowAttributesPanel(bool bShow)
	{
		attributesPanel.SetActive(bShow);
	}

	void OpenURL(string url)
	{
		switch(Application.platform) {
			case RuntimePlatform.WebGLPlayer: {
				url = string.Format("window.open(\"{0}\",\"_blank\")", url);
				Application.ExternalEval(url);
				break;
			}
			default: {
				Application.OpenURL(url);
				break;
			}
		}
	}
}