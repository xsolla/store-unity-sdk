using System;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;

public static class StoreDemoPopup
{
    public static void ShowSuccess(
        string message = "Everything happened as it should"
    ) =>
        PopupFactory.Instance.CreateSuccess().SetMessage(message);

    public static void ShowError(Error error)
    {
        Debug.LogError(error);
        PopupFactory.Instance.CreateError().SetMessage(error.ToString());
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
