using System;
using UnityEngine;

public class FriendActionsUI : MonoBehaviour
{
    [SerializeField] private GameObject actionPrefab;
    [SerializeField] private Transform actionContainer;
    
    private FriendButtonsUI _userButtons;

    private void AddAction(string actionName, Action callback)
    {
        if (actionPrefab != null && actionContainer != null)
        {
            var go = Instantiate(actionPrefab, actionContainer);
            var button = go.GetComponent<SimpleTextButton>();
            button.Text = actionName;
            button.onClick = callback;
        }
        else
        {
            Debug.LogError("In FriendActionsUI script `actionPrefab` or `actionContainer` = null!");
        }
    }

    public void Init(FriendModel friend, FriendButtonsUI userButtons)
    {
        _userButtons = userButtons;
        switch (friend.Relationship)
        {
            case UserRelationship.Friend:
            {
                var actions = gameObject.AddComponent<MyFriendActions>();
                actions.Init(friend, AddAction, _userButtons);
                break;
            }
            case UserRelationship.Blocked:
            {
                var actions = gameObject.AddComponent<BlockedUserActions>();
                actions.Init(friend, AddAction, _userButtons);
                break;
            }
        }
    }
}
