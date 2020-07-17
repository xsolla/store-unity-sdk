using System;
using UnityEngine;
using Xsolla.Core;

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
}
