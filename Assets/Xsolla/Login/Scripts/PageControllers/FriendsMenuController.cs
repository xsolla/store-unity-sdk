using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class FriendsMenuController : MonoBehaviour
	{
		[SerializeField] private GameObject userPrefab = default;
		[SerializeField] private GroupsController groupsController = default;
		[SerializeField] private ItemContainer usersContainer = default;
		[SerializeField] private InputField userSearchBox = default;

		[SerializeField] private Text EmptyGroupMessage = default;
		[SerializeField] private Text EmptyGroupSubMessage = default;
		[SerializeField] private SimpleButton AddFriendsButton = default;

		private void Awake()
		{
			gameObject.AddComponent<FriendsMenuHelper>();
			AddFriendsButton.onClick += () => groupsController.SelectGroup(FriendsMenuHelper.ADD_FRIENDS_GROUP);
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
						RefreshUsers(group, isOnGroupSelect: true);
					}
				};
				FriendsMenuHelper.RefreshGroups(groupsController);
				if (userSearchBox != null)
					userSearchBox.onValueChanged.AddListener(_ => RefreshUsers(groupsController.GetSelectedGroup().Name, isOnGroupSelect: false));
			}
		}

		private void RefreshUsers(string group, bool isOnGroupSelect)
		{
			var users = FriendsMenuHelper.GetUsersByGroup(group);
			users = FilterUsersByStartWord(users, userSearchBox.text);

			usersContainer.Clear();

			if (users != null && users.Count > 0)
			{
				if (isOnGroupSelect)
					userSearchBox.gameObject.SetActive(true);

				DisableEmptyGroupMessages();
				users.ForEach(u =>
				{
					var go = usersContainer.AddItem(userPrefab);
					go.GetComponent<FriendUI>().Initialize(u);
				});
			}
			else if (isOnGroupSelect)
			{
				userSearchBox.gameObject.SetActive(false);
				EnableEmptyGroupMessage(group);
			}
		}

		private List<FriendModel> FilterUsersByStartWord(List<FriendModel> users, string word)
		{
			return string.IsNullOrEmpty(word)
				? users
				: users.Where(u => u.Nickname.ToLower().StartsWith(word.ToLower())).ToList();
		}

		private void DisableEmptyGroupMessages()
		{
			EmptyGroupMessage.gameObject.SetActive(false);
			EmptyGroupSubMessage.gameObject.SetActive(false);
			AddFriendsButton.gameObject.SetActive(false);
		}

		private void EnableEmptyGroupMessage(string groupID)
		{
			string emptyMessage = null;
			string emptySubMessage = null;
			AddFriendsButton.gameObject.SetActive(false);

			switch (groupID)
			{
				case FriendsMenuHelper.USER_FRIENDS_GROUP:
					emptyMessage = "No friends yet";
					emptySubMessage = "Your friends appear here";
					AddFriendsButton.gameObject.SetActive(true);
					break;
				case FriendsMenuHelper.PENDING_USERS_GROUP:
					emptyMessage = "No pending friend requests";
					emptySubMessage = "Your friend requests appear here";
					break;
				case FriendsMenuHelper.REQUESTED_USERS_GROUP:
					emptyMessage = "No sent requests";
					emptySubMessage = "Your sent requests appear here";
					break;
				case FriendsMenuHelper.BLOCKED_USERS_GROUP:
					emptyMessage = "No blocked players";
					emptySubMessage = "All blocked players appear here";
					break;
				default:
					Debug.LogError($"Unknown groupID: {groupID}");
					return;
			}

			EmptyGroupMessage.text = emptyMessage;
			EmptyGroupSubMessage.text = emptySubMessage;

			EmptyGroupMessage.gameObject.SetActive(true);
			EmptyGroupSubMessage.gameObject.SetActive(true);
		}
	}
}
