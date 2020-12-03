using System;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;
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
				var linkingResult = new LinkingResultContainer();
				PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => linkingResult.IsLinked != null);

				DemoController.Instance.GetImplementation().LinkConsoleAccount(
					userId: XsollaSettings.UsernameFromConsolePlatform,
					platform: XsollaSettings.Platform.GetString(),
					confirmationCode: code,
					onSuccess: () => LinkingAccountHandler(linkingResult),
					onError: error => { linkingResult.IsLinked = false; StoreDemoPopup.ShowError(error); });
			});
		};
	}

	private void LinkingAccountHandler(LinkingResultContainer linkingResult)
	{
		DemoController.Instance.GetImplementation().SignInConsoleAccount(
			userId: XsollaSettings.UsernameFromConsolePlatform,
			platform: XsollaSettings.Platform.GetString(),
			successCase: newToken => OnSuccessConsoleLogin(newToken, linkingResult),
			failedCase: error => { linkingResult.IsLinked = false; StoreDemoPopup.ShowError(error); }
		);
	}

	private void OnSuccessConsoleLogin(string newToken, LinkingResultContainer linkingResult)
	{
		DemoController.Instance.GetImplementation().ValidateToken(
			token: newToken,
			onSuccess: validToken => ApplyNewToken(validToken, linkingResult),
			onError: error => { linkingResult.IsLinked = false; StoreDemoPopup.ShowError(error); });
	}

	private void ApplyNewToken(string newToken, LinkingResultContainer linkingResult)
	{
		DemoController.Instance.GetImplementation().Token = newToken;
		UserInventory.Instance.Refresh(onSuccess: () => GoToInventory(linkingResult), onError: error => { linkingResult.IsLinked = false; StoreDemoPopup.ShowError(error); });
	}

	private void GoToInventory(LinkingResultContainer linkingResult)
	{
		linkingResult.IsLinked = true;
		DemoController.Instance.SetState(MenuState.Inventory);
		PopupFactory.Instance.CreateSuccess();
	}

	private void ShowCodeConfirmation(Action<string> callback)
	{
		PopupFactory.Instance.CreateCodeConfirmation().SetConfirmCallback(callback);
	}

	private class LinkingResultContainer
	{
		public bool? IsLinked;
	}
}