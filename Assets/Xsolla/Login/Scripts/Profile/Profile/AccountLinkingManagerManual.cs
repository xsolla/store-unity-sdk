using System;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;

namespace Xsolla.Demo
{
	public class AccountLinkingManagerManual : MonoBehaviour
	{
		[SerializeField] private SimpleButton accountLinkingButton = default;
		[SerializeField] private SimpleButton getAccountLinkButton = default;

		public void Init()
		{
			if (DemoController.Instance.LoginDemo.Token.IsMasterAccount())
				GetAccountLinkButtonEnable();
			else
				LinkingButtonEnable();
		}

		private void GetAccountLinkButtonEnable()
		{
			getAccountLinkButton.onClick += () =>
			{
				DemoController.Instance.LoginDemo.RequestLinkingCode(
					code => StoreDemoPopup.ShowSuccess($"YOUR CODE: {code.code}"),
					StoreDemoPopup.ShowError);
			};

			accountLinkingButton.transform.parent.gameObject.SetActive(false);
			getAccountLinkButton.transform.parent.gameObject.SetActive(true);
		}

		private void LinkingButtonEnable()
		{
			accountLinkingButton.onClick += () =>
			{
				ShowCodeConfirmation(code =>
				{
					var linkingResult = new LinkingResultContainer();
					PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => linkingResult.IsLinked != null);

					DemoController.Instance.LoginDemo.LinkConsoleAccount(
						userId: XsollaSettings.UsernameFromConsolePlatform,
						platform: XsollaSettings.Platform.GetString(),
						confirmationCode: code,
						onSuccess: () => LinkingAccountHandler(linkingResult),
						onError: error => { linkingResult.IsLinked = false; StoreDemoPopup.ShowError(error); });
				});
			};

			getAccountLinkButton.transform.parent.gameObject.SetActive(false);
			accountLinkingButton.transform.parent.gameObject.SetActive(true);
		}

		private void LinkingAccountHandler(LinkingResultContainer linkingResult)
		{
			DemoController.Instance.LoginDemo.SignInConsoleAccount(
				userId: XsollaSettings.UsernameFromConsolePlatform,
				platform: XsollaSettings.Platform.GetString(),
				successCase: newToken => OnSuccessConsoleLogin(newToken, linkingResult),
				failedCase: error => { linkingResult.IsLinked = false; StoreDemoPopup.ShowError(error); }
			);
		}

		private void OnSuccessConsoleLogin(string newToken, LinkingResultContainer linkingResult)
		{
			DemoController.Instance.LoginDemo.ValidateToken(
				token: newToken,
				onSuccess: validToken => ApplyNewToken(validToken, linkingResult),
				onError: error => { linkingResult.IsLinked = false; StoreDemoPopup.ShowError(error); });
		}

		private void ApplyNewToken(string newToken, LinkingResultContainer linkingResult)
		{
			DemoController.Instance.LoginDemo.Token = newToken;

			if (DemoController.Instance.InventoryDemo == null)
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
