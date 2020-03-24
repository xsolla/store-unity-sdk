using System;

namespace Xsolla.Core.Popup
{
	public interface IErrorPopup
	{
		IErrorPopup SetCallback(Action buttonPressed);
		IErrorPopup SetTitle(string titleText);
		IErrorPopup SetMessage(string messageText);
		IErrorPopup SetButtonText(string buttonText);
	}
}