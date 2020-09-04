using System;

public class UserStateMyFriend : BaseUserStateUI
{
    protected override void InitUserButtons(FriendButtonsUI buttons)
    { }

    protected override void InitUserActionsButton(FriendActionsButton actionsButton)
    {
        EnableBlockUserOption();
        EnableDeleteUserOption();
    }
}
