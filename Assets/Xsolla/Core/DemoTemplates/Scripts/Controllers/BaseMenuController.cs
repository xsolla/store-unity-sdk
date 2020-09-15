using System;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;

public abstract class BaseMenuController : MonoBehaviour
{
	protected static void AttachButtonCallback(SimpleButton button, Action callback)
	{
		if (button != null && callback != null)
		{
			button.onClick = callback;
		}
	}

	protected static void AttachUrlToButton(SimpleButton button, string url)
	{
		if (!string.IsNullOrEmpty(url))
		{
			AttachButtonCallback(button, () => BrowserHelper.Instance.Open(url));
		}
	}
	
	protected static void SetMenuState(MenuState state, Func<bool> condition = null)
	{
		if (condition == null || condition.Invoke())
			DemoController.Instance.SetState(state);
		else
		{
			PopupFactory.Instance.CreateWaiting()
				.SetCloseCondition(condition)
				.SetCloseHandler(() => DemoController.Instance.SetState(state));
		}
	}
}
