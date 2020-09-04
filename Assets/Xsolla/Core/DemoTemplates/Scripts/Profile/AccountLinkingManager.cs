using System;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;
using Xsolla.Login;
using Xsolla.Store;

public class AccountLinkingManager : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] private SimpleButton accountLinkingButton;
	[SerializeField] private SimpleButton getAccountLinkButton;
#pragma warning restore 0649

	private void Start()
	{
		if (DemoController.Instance.GetImplementation().Token.IsMasterAccount())
			GetAccountLinkButtonEnable();
		else
			LinkingButtonEnable();
	}

	private void GetAccountLinkButtonEnable()
	{
		accountLinkingButton.gameObject.SetActive(false);
		getAccountLinkButton.gameObject.SetActive(true);

		getAccountLinkButton.onClick += () =>
		{
			DemoController.Instance.GetImplementation().RequestLinkingCode(
				code => StoreDemoPopup.ShowSuccess($"YOUR CODE: {code.code}"),
				StoreDemoPopup.ShowError);
		};
	}

	private void LinkingButtonEnable()
	{
		getAccountLinkButton.gameObject.SetActive(false);
		accountLinkingButton.gameObject.SetActive(true);
		
		accountLinkingButton.onClick += () =>
		{
			ShowCodeConfirmation(code =>
			{
				DemoController.Instance.GetImplementation().LinkConsoleAccount(
					userId: XsollaSettings.UsernameFromConsolePlatform,
					platform: XsollaSettings.Platform.GetString(),
					confirmationCode: code,
					onSuccess: LinkingAccountHandler,
					onError: StoreDemoPopup.ShowError);
			});
		};
	}

	private void LinkingAccountHandler()
	{
		PopupFactory.Instance.CreateSuccess();
		DemoController.Instance.GetImplementation().SignInConsoleAccount(
			userId: XsollaSettings.UsernameFromConsolePlatform,
			platform: XsollaSettings.Platform.GetString(),
			successCase: OnSuccessConsoleLogin,
			failedCase: StoreDemoPopup.ShowError
		);
	}

	private void OnSuccessConsoleLogin(string newToken)
	{
		DemoController.Instance.GetImplementation().ValidateToken(
			token: newToken,
			onSuccess: validToken => ApplyNewToken(validToken),
			onError: StoreDemoPopup.ShowError);
	}

	private void ApplyNewToken(string newToken)
	{
		DemoController.Instance.GetImplementation().Token = newToken;
		DemoController.Instance.SetState(MenuState.Main);
		UserInventory.Instance.Refresh(onSuccess: GoToInventory, onError: StoreDemoPopup.ShowError);
	}

	private void GoToInventory()
	{
		DemoController.Instance.SetState(MenuState.Inventory);
	}

	private void ShowCodeConfirmation(Action<string> callback)
	{
		PopupFactory.Instance.CreateCodeConfirmation().SetConfirmCallback(callback);
	}
}