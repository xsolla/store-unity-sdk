using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FriendsMenuController : MonoBehaviour
{
    private const string USER_FRIENDS_GROUP = "MY FRIENDS";
    private const string PENDING_USERS_GROUP = "PENDING";
    private const string REQUESTED_USERS_GROUP = "REQUESTS";
    private const string BLOCKED_USERS_GROUP = "BLOCKED";
    
    [SerializeField] private GameObject userPrefab;
    [SerializeField] private GroupsController groupsController;
    [SerializeField] private ItemContainer usersContainer;
    [SerializeField] private InputField userSearchBox;

    private void Awake()
    {
        UserFriends.Instance.StopRefreshUsers();
    }

    private void OnDestroy()
    {
        UserFriends.Instance.StartRefreshUsers();
    }

    void Start()
    {
        if (groupsController != null)
        {
            groupsController.GroupSelectedEvent += RefreshUsers;
            RefreshGroups();
            if (userSearchBox != null)
                userSearchBox.onValueChanged.AddListener(_ => RefreshUsers(groupsController.GetSelectedGroup().Name));
        }
    }

    private void RefreshGroups()
    {
        if(groupsController == null) return;
        groupsController.RemoveAll();
        
        var friendsGroup = groupsController.AddGroup(USER_FRIENDS_GROUP).GetComponent<FriendGroup>();
        friendsGroup.SetUsersCount(UserFriends.Instance.Friends.Count);
        UserFriends.Instance.UserFriendsUpdatedEvent += () => friendsGroup.SetUsersCount(UserFriends.Instance.Friends.Count);
        
        var pendingGroup = groupsController.AddGroup(PENDING_USERS_GROUP).GetComponent<FriendGroup>();
        pendingGroup.SetUsersCount(UserFriends.Instance.Pending.Count);
        UserFriends.Instance.PendingUsersUpdatedEvent += () => pendingGroup.SetUsersCount(UserFriends.Instance.Pending.Count);
        
        var requestedGroup = groupsController.AddGroup(REQUESTED_USERS_GROUP).GetComponent<FriendGroup>();
        requestedGroup.SetUsersCount(UserFriends.Instance.Requested.Count);
        UserFriends.Instance.RequestedUsersUpdatedEvent += () => requestedGroup.SetUsersCount(UserFriends.Instance.Requested.Count);
        
        var blockedGroup = groupsController.AddGroup(BLOCKED_USERS_GROUP).GetComponent<FriendGroup>();
        blockedGroup.SetUsersCount(UserFriends.Instance.Blocked.Count);
        UserFriends.Instance.BlockedUsersUpdatedEvent += () => blockedGroup.SetUsersCount(UserFriends.Instance.Blocked.Count);
        
        groupsController.SelectDefault();
    }

    private void RefreshUsers(string group)
    {
        var users = GetUsersByGroup(group);
        usersContainer.Clear();
        
        FilterUsersByStartWord(users, userSearchBox.text).ForEach(u =>
        {
            var go = usersContainer.AddItem(userPrefab);
            go.GetComponent<FriendUI>().Initialize(u);
        });
    }

    private List<FriendModel> FilterUsersByStartWord(List<FriendModel> users, string word)
    {
        return string.IsNullOrEmpty(word)
            ? users
            : users.Where(u => u.Nickname.ToLower().StartsWith(word.ToLower())).ToList();
    }

    private List<FriendModel> GetUsersByGroup(string group)
    {
        group = group.Split('(')[0].Trim();
        switch (group)
        {
            case USER_FRIENDS_GROUP: return UserFriends.Instance.Friends;
            case PENDING_USERS_GROUP: return UserFriends.Instance.Pending;
            case REQUESTED_USERS_GROUP: return UserFriends.Instance.Requested;
            case BLOCKED_USERS_GROUP: return UserFriends.Instance.Blocked;
            default: return new List<FriendModel>();
        }
    }
}
