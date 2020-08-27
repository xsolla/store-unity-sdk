using System;

public class UserStateMyFriend : BaseUserStateUI
{
    // private const string BLOCK_USER_OPTION = "BLOCK USER";
    // private const string DELETE_USER_OPTION = "DELETE USER";
    
    protected override void InitUserButtons(FriendButtonsUI buttons)
    { }

    protected override void InitUserActionsButton(FriendActionsButton actionsButton)
    {
        EnableBlockUserOption();
        EnableDeleteUserOption();
        // actionsButton.AddAction(BLOCK_USER_OPTION, () =>
        // {
        //     SetState(UserState.Blocked);
        // });
        // actionsButton.AddAction(DELETE_USER_OPTION, () =>
        // {
        //     SetState(UserState.Initial);
        // });
    }
}
