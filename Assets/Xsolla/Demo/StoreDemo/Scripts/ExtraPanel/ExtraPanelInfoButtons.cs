using System;
using UnityEngine;

[AddComponentMenu("Scripts/Xsolla.Store/Extra/ExtraPanelInfoButtons")]
public class ExtraPanelInfoButtons : MonoBehaviour
{
    const string URL_REGISTRATION = "https://publisher.xsolla.com/signup?store_type=sdk";
    const string URL_INSTRUCTION = "https://developers.xsolla.com/sdk/game-engines/unity/";
    const string URL_TEST_CARDS = "https://developers.xsolla.com/api/v1/pay-station/#api_payment_ui_test_cards";

    public event Action<string> OpenUrlEvent;

    [SerializeField]
    GameObject registrationButton;

    [SerializeField]
    GameObject instructionButton;

    [SerializeField]
    GameObject testCardsButton;

    public void Init()
    {
        EnableInfoButton(registrationButton, URL_REGISTRATION);
        EnableInfoButton(instructionButton, URL_INSTRUCTION);
        EnableInfoButton(testCardsButton, URL_TEST_CARDS);
    }

	private void EnableInfoButton(GameObject go, string url)
	{
        go.SetActive(true);
        var buttonComponent = go.GetComponent<SimpleTextButton>();
		if(buttonComponent)
			buttonComponent.onClick = () => OpenUrlEvent?.Invoke(url);
    }
}
