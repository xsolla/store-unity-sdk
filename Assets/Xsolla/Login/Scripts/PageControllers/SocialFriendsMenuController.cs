using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class SocialFriendsMenuController : MonoBehaviour
	{
		[SerializeField] private GameObject userPrefab = default;
		[SerializeField] private ItemContainer usersContainer = default;
		[SerializeField] private FriendSystemSocialNetwork[] SocialNetworks = default;

		private void Awake()
		{
			foreach (var network in SocialNetworks)
				network.StateChanged += NetworkOnStateChanged;
		}

		private void NetworkOnStateChanged(SocialProvider socialProvider, FriendSystemSocialNetwork.State state)
		{
			DemoController.Instance.LoginDemo.GetFriendsFromSocialNetworks(RefreshUsersContainer);
		}

		private void RefreshUsersContainer(List<FriendModel> users)
		{
			var addedUsers = new List<FriendModel>();
			var createdFriendUIs = new List<FriendUI>();

			usersContainer.Clear();
			users.ForEach(newUser =>
			{
				int index;
				if ((index = addedUsers.IndexOf(newUser)) != -1)//This will be true for Xsolla users with several social networks linked
				{
					createdFriendUIs[index].AddSocialFriendshipMark(newUser.SocialProvider);
				}
				else
				{
					var FriendUIgameObject = usersContainer.AddItem(userPrefab);
					var friendUIscript = FriendUIgameObject.GetComponent<FriendUI>();
					friendUIscript.Initialize(newUser);

					addedUsers.Add(newUser);
					createdFriendUIs.Add(friendUIscript);
				}
			});

			if(usersContainer.IsEmpty())
				usersContainer.EnableEmptyContainerMessage();
			else
				usersContainer.DisableEmptyContainerMessage();
		}
	}
}
