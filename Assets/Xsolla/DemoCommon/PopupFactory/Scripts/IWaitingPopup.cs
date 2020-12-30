using System;

namespace Xsolla.Core.Popup
{
	public interface IWaitingPopup
	{
		IWaitingPopup SetCloseCondition(Func<bool> condition);
		IWaitingPopup SetCloseHandler(Action handler);
	}
}