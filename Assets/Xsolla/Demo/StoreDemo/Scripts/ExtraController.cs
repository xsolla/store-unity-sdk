using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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

	AttributesSidePanel attributesSidePanel;

	const string URL_REGISTRATION = "https://publisher.xsolla.com/signup?store_type=sdk";
	const string URL_INSTRUCTION = "https://developers.xsolla.com/sdk/game-engines/unity/";
	const string URL_TEST_CARDS = "https://developers.xsolla.com/api/v1/pay-station/#api_payment_ui_test_cards";

	private void Awake()
	{
		attributesSidePanel = attributesPanel.GetComponent<AttributesSidePanel>();
	}

	public void Init()
	{
		signOutButton.SetActive(true);
		registrationButton.SetActive(true);
		instructionButton.SetActive(true);
		testCardsButton.SetActive(true);

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