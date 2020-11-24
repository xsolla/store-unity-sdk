using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FriendsMenuController : MonoBehaviour
{
    [SerializeField] private GameObject userPrefab;
    [SerializeField] private GroupsController groupsController;
    [SerializeField] private ItemContainer usersContainer;
    [SerializeField] private InputField userSearchBox;
    
    private void Awake()
    {
        gameObject.AddComponent<FriendsMenuHelper>();
    }

    void Start()
    {
        if (groupsController != null)
        {
            groupsController.GroupSelectedEvent += group =>
            {
                if (group.Equals(FriendsMenuHelper.ADD_FRIENDS_GROUP))
                    DemoController.Instance.SetState(MenuState.SocialFriends);
                else
                {
                    userSearchBox.text = string.Empty;
                    RefreshUsers(group);    
                }
            };
            FriendsMenuHelper.RefreshGroups(groupsController);
            if (userSearchBox != null)
                userSearchBox.onValueChanged.AddListener(_ => RefreshUsers(groupsController.GetSelectedGroup().Name));
        }
    }
    
    private void RefreshUsers(string group)
    {
        var users = FriendsMenuHelper.GetUsersByGroup(group);
        users = FilterUsersByStartWord(users, userSearchBox.text);
        RefreshUsersContainer(users);
    }

    private void RefreshUsersContainer(List<FriendModel> users)
    {
        usersContainer.Clear();
        users.ForEach(u =>
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
}
