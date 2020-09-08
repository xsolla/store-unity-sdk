using System;

namespace Xsolla.Core.Popup
{
	public interface ISuccessPopup
	{
		ISuccessPopup SetCallback(Action buttonPressed);
		ISuccessPopup SetTitle(string titleText);
		ISuccessPopup SetMessage(string messageText);
		ISuccessPopup SetButtonText(string buttonText);
	}
}