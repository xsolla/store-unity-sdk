using System;
using UnityEngine;

public class MyFriendActions : MonoBehaviour
{
    private const string REMOVE_FRIEND_OPTION = "REMOVE FRIEND";
    private const string BLOCK_FRIEND_OPTION = "BLOCK FRIEND";

    private FriendModel _friend;
    private FriendButtonsUI _userButtons;

    public void Init(FriendModel friend, Action<string, Action> addActionMethod, FriendButtonsUI userButtons)
    {
        _friend = friend;
        _userButtons = userButtons;
        
        addActionMethod?.Invoke(REMOVE_FRIEND_OPTION, RemoveFriendButtonClick);
        addActionMethod?.Invoke(BLOCK_FRIEND_OPTION, BlockFriendButtonClick);
        
        if(userButtons != null)
            userButtons.ClearButtons();
    }

    private void RemoveFriendButtonClick()
    {
        if(_friend == null) return;
        UserFriends.Instance.RemoveFriend(_friend);
    }
    
    private void BlockFriendButtonClick()
    {
        if(_friend == null) return;
        UserFriends.Instance.BlockUser(_friend);
    }
}
