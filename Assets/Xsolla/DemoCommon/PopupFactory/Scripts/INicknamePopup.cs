using System;

namespace Xsolla.Core.Popup
{
	public interface INicknamePopup
	{
		INicknamePopup SetCallback(Action<string> nicknameCallback);
		INicknamePopup SetCancelCallback(Action cancelCallback);
	}
}