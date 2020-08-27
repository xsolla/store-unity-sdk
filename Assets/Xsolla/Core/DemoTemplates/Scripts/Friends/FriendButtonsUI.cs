using UnityEngine;

public class FriendButtonsUI : MonoBehaviour
{
    [SerializeField] private SimpleTextButton addFriendButton;
    [SerializeField] private SimpleTextButton acceptButton;
    [SerializeField] private SimpleTextButton declineButton;
    [SerializeField] private SimpleTextButton cancelRequestButton;
    [SerializeField] private SimpleTextButton unblockButton;

    private void Awake()
    {
        DisableAll();
    }

    private SimpleTextButton SetButtonVisibility(SimpleTextButton button, bool isVisible)
    {
        if(button != null)
            button.gameObject.SetActive(isVisible);
        return button;
    }

    public void DisableAll()
    {
        SetButtonVisibility(addFriendButton, false);
        SetButtonVisibility(acceptButton, false);
        SetButtonVisibility(declineButton, false);
        SetButtonVisibility(cancelRequestButton, false);
        SetButtonVisibility(unblockButton, false);
    }

    public SimpleTextButton EnableAddFriendButton()
    {
        return SetButtonVisibility(addFriendButton, true);
    }

    public SimpleTextButton EnableAcceptButton()
    {
        return SetButtonVisibility(acceptButton, true);
    }
    
    public SimpleTextButton EnableDeclineButton()
    {
        return SetButtonVisibility(declineButton, true);
    }
    
    public SimpleTextButton EnableCancelRequestButton()
    {
        return SetButtonVisibility(cancelRequestButton, true);
    }
    
    public SimpleTextButton EnableUnblockButton()
    {
        return SetButtonVisibility(unblockButton, true);
    }
}
