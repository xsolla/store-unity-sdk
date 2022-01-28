using System;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;

namespace Xsolla.Demo
{
	public class AccountLinkingManager : MonoBehaviour
	{
		[SerializeField] private SimpleButton accountLinkingButton = default;
		[SerializeField] private SimpleButton getAccountLinkButton = default;

		private void Start()
		{
			if (Token.Instance.IsMasterAccount())
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
				SdkLoginLogic.Instance.RequestLinkingCode(
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

					SdkLoginLogic.Instance.LinkConsoleAccount(
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
			SdkLoginLogic.Instance.SignInConsoleAccount(
				userId: XsollaSettings.UsernameFromConsolePlatform,
				platform: XsollaSettings.Platform.GetString(),
				onSuccess: newToken => OnSuccessConsoleLogin(newToken, linkingResult),
				onError: error => { linkingResult.IsLinked = false; StoreDemoPopup.ShowError(error); }
			);
		}

		private void OnSuccessConsoleLogin(string newToken, LinkingResultContainer linkingResult)
		{
			SdkLoginLogic.Instance.ValidateToken(
				token: newToken,
				onSuccess: validToken => ApplyNewToken(validToken, linkingResult),
				onError: error => { linkingResult.IsLinked = false; StoreDemoPopup.ShowError(error); });
		}

		private void ApplyNewToken(string newToken, LinkingResultContainer linkingResult)
		{
			Token.Instance = Token.Create(newToken);

			if (!DemoMarker.IsInventoryPartAvailable)
				FindObjectOfType<UserInfoDrawer>()?.Refresh();
			else
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
}
