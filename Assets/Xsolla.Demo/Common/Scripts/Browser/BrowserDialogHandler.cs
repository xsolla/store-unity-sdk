using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;

namespace Xsolla.Demo
{
	public class BrowserDialogHandler : MonoBehaviour
	{
		private void Awake()
		{
			var browser = BrowserHelper.Instance.InAppBrowser;
			if (browser == null)
				return;
			
			browser.AlertDialogEvent += (message, acceptAction) =>
			{
				PopupFactory.Instance.CreateSuccess()
					.SetTitle("Attention")
					.SetMessage(message)
					.SetCallback(() => acceptAction?.Invoke());
			};

			browser.ConfirmDialogEvent += (message, acceptAction, cancelAction) =>
			{
				PopupFactory.Instance.CreateConfirmation()
					.SetMessage(message)
					.SetConfirmCallback(() => acceptAction?.Invoke())
					.SetCancelCallback(() => cancelAction?.Invoke());
			};
		}
	}
}