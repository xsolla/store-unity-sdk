using System;
using UnityEngine;

public class PendingUserActions : MonoBehaviour
{
    private const string UNBLOCK_USER_OPTION = "UNBLOCK USER";

    private FriendModel _friend;
    private FriendButtonsUI _userButtons;

    public void Init(FriendModel friend, Action<string, Action> addActionMethod, FriendButtonsUI userButtons)
    {
        _friend = friend;
        _userButtons = userButtons;
        
        //addActionMethod?.Invoke(UNBLOCK_USER_OPTION, UnblockUserButtonClick);
        if(_userButtons != null)
            _userButtons.EnableForPendingUser();
    }

    // private void UnblockUserButtonClick()
    // {
    //     if(_friend == null) return;
    //     UserFriends.Instance.UnblockUser(_friend);
    // }
}
