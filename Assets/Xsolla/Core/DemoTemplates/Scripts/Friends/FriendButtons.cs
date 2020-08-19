using System;
using System.Collections.Generic;
using UnityEngine;

public class FriendButtonsUI : MonoBehaviour
{
    [SerializeField] private SimpleTextButton addFriendButton;
    [SerializeField] private SimpleTextButton acceptButton;
    [SerializeField] private SimpleTextButton declineButton;
    [SerializeField] private SimpleTextButton cancelRequestButton;
    [SerializeField] private SimpleTextButton unblockButton;

    private void Start()
    {
        ClearButtons();
    }

    private void SetButtonVisibility(SimpleTextButton button, bool isVisible)
    {
        if(button != null)
            button.gameObject.SetActive(isVisible);
    }

    private void EnableButtons(List<SimpleTextButton> buttons)
    {
        ClearButtons();
        buttons.ForEach(b => SetButtonVisibility(b, true));
    }
    
    public void ClearButtons()
    {
        SetButtonVisibility(addFriendButton, false);
        SetButtonVisibility(acceptButton, false);
        SetButtonVisibility(declineButton, false);
        SetButtonVisibility(cancelRequestButton, false);
        SetButtonVisibility(unblockButton, false);
    }

    public void EnableForPendingUser()
    {
        EnableButtons(new List<SimpleTextButton>{acceptButton, declineButton});
    }

    public void EnableForRequestedUser()
    {
        SetButtonVisibility(cancelRequestButton, true);
    }
    
    public void EnableForBlockedUser()
    {
        SetButtonVisibility(unblockButton, true);
    }
}
