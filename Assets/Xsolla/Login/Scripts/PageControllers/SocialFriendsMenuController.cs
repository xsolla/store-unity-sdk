using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class SocialFriendsMenuController : MonoBehaviour
	{
		[SerializeField] private GameObject userPrefab;
		[SerializeField] private ItemContainer usersContainer;
		[SerializeField] private FriendSystemSocialNetwork[] SocialNetworks;

		private void Awake()
		{
			foreach (var network in SocialNetworks)
				network.StateChanged += NetworkOnStateChanged;
		}

		private void NetworkOnStateChanged(SocialProvider socialProvider, FriendSystemSocialNetwork.State state)
		{
			SdkFriendsLogic.Instance.GetFriendsFromSocialNetworks(RefreshUsersContainer);
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
					var friendUiGameObject = usersContainer.AddItem(userPrefab);
					var friendUiScript = friendUiGameObject.GetComponent<FriendUI>();
					friendUiScript.Initialize(newUser);

					addedUsers.Add(newUser);
					createdFriendUIs.Add(friendUiScript);
				}
			});

			if(usersContainer.IsEmpty())
				usersContainer.EnableEmptyContainerMessage();
			else
				usersContainer.DisableEmptyContainerMessage();
		}
	}
}
