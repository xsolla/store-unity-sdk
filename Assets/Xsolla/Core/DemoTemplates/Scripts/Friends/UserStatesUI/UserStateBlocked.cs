using System;

public class UserStateBlocked : BaseUserStateUI
{
    //private const string UNBLOCK_USER_OPTION = "UNBLOCK USER";
    
    protected override void InitUserButtons(FriendButtonsUI buttons)
    {
        EnableUnblockUserButton();
        // buttons.EnableUnblockButton().onClick = () =>
        // {
        //     SetState(UserState.Initial);
        // };
    }

    protected override void InitUserActionsButton(FriendActionsButton actionsButton)
    {
        actionsButton.gameObject.SetActive(false);
    }
}
