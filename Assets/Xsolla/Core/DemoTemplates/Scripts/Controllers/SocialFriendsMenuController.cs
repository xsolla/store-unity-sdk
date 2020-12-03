using System.Collections.Generic;
using UnityEngine;

public class SocialFriendsMenuController : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] private GameObject userPrefab;
    [SerializeField] private ItemContainer usersContainer;
	[SerializeField] private FriendSystemSocialNetwork[] SocialNetworks;
#pragma warning restore 0649

	private void Awake()
	{
		foreach (var network in SocialNetworks)
			network.StateChanged += NetworkOnStateChanged;
	}

	private void NetworkOnStateChanged(SocialProvider socialProvider, FriendSystemSocialNetwork.State state)
	{
		DemoController.Instance.GetImplementation().GetFriendsFromSocialNetworks(RefreshUsersContainer);
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
