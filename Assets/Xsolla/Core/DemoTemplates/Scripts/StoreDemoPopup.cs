using System;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;

namespace Xsolla.Store
{
    public static class StoreDemoPopup
    {
        public static void ShowSuccess(string message = "") => PopupFactory.Instance.CreateSuccess().SetMessage(message);

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
            PopupFactory.Instance.CreateConfirmation().
                SetMessage(message).
                SetConfirmCallback(confirmCase).
                SetCancelCallback(cancelCase);
    }
}
