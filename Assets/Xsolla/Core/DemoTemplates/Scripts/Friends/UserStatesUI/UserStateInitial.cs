public class UserStateInitial : BaseUserStateUI
{
    //private const string BLOCK_USER_OPTION = "BLOCK USER";
    protected override void InitUserButtons(FriendButtonsUI buttons)
    {
        EnableAddFriendButton();
        // buttons.EnableAddFriendButton().onClick = () =>
        // {
        //     SetState(UserState.Requested);
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
