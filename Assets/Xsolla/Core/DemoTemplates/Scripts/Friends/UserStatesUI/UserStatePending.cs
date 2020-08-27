using System;

public class UserStatePending : BaseUserStateUI
{
    //private const string BLOCK_USER_OPTION = "BLOCK USER";
    
    protected override void InitUserButtons(FriendButtonsUI buttons)
    {
        EnableAcceptFriendshipButton();
        EnableDeclineFriendshipButton();
        // buttons.EnableAcceptButton().onClick = () =>
        // {
        //     SetState(UserState.MyFriend);
        // };
        // buttons.EnableDeclineButton().onClick = () =>
        // {
        //     SetState(UserState.Initial);
        // };
    }

    protected override void InitUserActionsButton(FriendActionsButton actionsButton)
    {
        EnableBlockUserOption();
        // actionsButton.AddAction(BLOCK_USER_OPTION, () =>
        // {
        //     SetState(UserState.Blocked);
        // });
    }
}
