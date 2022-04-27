using System;
using Xsolla.Core;
using Xsolla.Core.Popup;

namespace Xsolla.Demo
{
	public static class StoreDemoPopup
	{
		public static void ShowSuccess(string message = "Everything happened as it should", Action buttonCallback = null)
		{
			var popup = PopupFactory.Instance.CreateSuccess();
			popup.SetMessage(message);
			popup.SetCallback(buttonCallback);
		}

		public static void ShowError(Error error)
		{
			Debug.LogError(error);
			var popup = PopupFactory.Instance.CreateError();
			popup.SetMessage(error.ToString());
		}

		public static void ShowError(Error error, Action buttonCallback)
		{
			Debug.LogError(error);
			var popup = PopupFactory.Instance.CreateError();
			popup.SetMessage(error.ToString());
			popup.SetCallback(buttonCallback);
		}

		public static void ShowWarning(Error error, Action buttonCallback = null)
		{
			Debug.LogWarning(error);
			var popup = PopupFactory.Instance.CreateError();
			popup.SetMessage(error.ToString());

			if (buttonCallback != null)
				popup.SetCallback(buttonCallback);
		}

		public static void ShowConfirm(
			Action confirmCase,
			Action cancelCase = null,
			string message = "Are you sure you want to buy this item?"
		) => 
			PopupFactory.Instance.CreateConfirmation()
				.SetMessage(message)
				.SetConfirmCallback(confirmCase)
				.SetCancelCallback(cancelCase);

		public static void ShowConsumeConfirmation(
			string itemName,
			uint quantity,
			Action confirmCase,
			Action cancelCase = null
		) => 
			PopupFactory.Instance.CreateConfirmation()
				.SetMessage(
					$"Item{(quantity > 1 ? "s" : "")} '{itemName}' x {quantity} will be consumed. Are you sure?")
				.SetConfirmCallback(confirmCase)
				.SetCancelCallback(cancelCase);
	}
}
