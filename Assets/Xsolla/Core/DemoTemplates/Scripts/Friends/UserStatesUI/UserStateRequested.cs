public class UserStateRequested : BaseUserStateUI
{
    //private const string BLOCK_USER_OPTION = "BLOCK USER";

    protected override void InitUserButtons(FriendButtonsUI buttons)
    {
        EnableCancelFriendshipRequestButton();
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
