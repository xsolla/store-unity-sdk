using System.Collections.Generic;
using UnityEngine;
using Xsolla.Store;

public class StoreController : MonoBehaviour
{
	[SerializeField]
	GroupsController _groupsController;
	[SerializeField]
	ItemsController _itemsController;

	XsollaStore _store;

	public static Dictionary<string, Sprite> ItemIcons;

	public Cart Cart { get; private set; }


	public List<string> cartItems;

	private void Start()
	{
		ItemIcons = new Dictionary<string, Sprite>();

		cartItems = new List<string>();

		_store = XsollaStore.Instance;

		_store.Token =
			"eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJleHAiOjE5NjIyMzQwNDgsImlzcyI6Imh0dHBzOi8vbG9naW4ueHNvbGxhLmNvbSIsImlhdCI6MTU2MjE0NzY0OCwidXNlcm5hbWUiOiJ4c29sbGEiLCJ4c29sbGFfbG9naW5fYWNjZXNzX2tleSI6IjA2SWF2ZHpDeEVHbm5aMTlpLUc5TmMxVWFfTWFZOXhTR3ZEVEY4OFE3RnMiLCJzdWIiOiJkMzQyZGFkMi05ZDU5LTExZTktYTM4NC00MjAxMGFhODAwM2YiLCJlbWFpbCI6InN1cHBvcnRAeHNvbGxhLmNvbSIsInR5cGUiOiJ4c29sbGFfbG9naW4iLCJ4c29sbGFfbG9naW5fcHJvamVjdF9pZCI6ImU2ZGZhYWM2LTc4YTgtMTFlOS05MjQ0LTQyMDEwYWE4MDAwNCIsInB1Ymxpc2hlcl9pZCI6MTU5MjR9.GCrW42OguZbLZTaoixCZgAeNLGH2xCeJHxl8u8Xn2aI";

		SubscribeToEvents();

		_store.CreateNewCart(newCart => { Cart = newCart; }, error => { print(error.ToString()); });

		_store.GetListOfItems((items) =>
		{
			CreateGroups(items);
			CreateItems(items);
		}, error => { Debug.Log(error.ToString()); });
	}

	void SubscribeToEvents()
	{
		_groupsController.OnGroupClick += (id) =>
		{
			_itemsController.ActivateContainer(id);
			_groupsController.ChangeSelection(id);
		};
	}

	void CreateGroups(StoreItems items)
	{
		List<string> addedGroups = new List<string>();
		foreach (var item in items.items)
		{
			foreach (string groupName in item.groups)
			{
				if (!addedGroups.Contains(groupName))
				{
					addedGroups.Add(groupName);
					_groupsController.AddGroup(groupName);
				}
			}
		}

		_groupsController.AddCart();
	}

	void CreateItems(StoreItems items)
	{
		foreach (var item in items.items)
		{
			_itemsController.AddItem(item);
		}

		_itemsController.CreateCart();
	}
}