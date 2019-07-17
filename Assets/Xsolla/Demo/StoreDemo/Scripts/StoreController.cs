﻿using System.Collections.Generic;
using UnityEngine;
using Xsolla.Store;

public class StoreController : MonoBehaviour
{
	GroupsController _groupsController;
	ItemsController _itemsController;

	public CartModel CartModel { get; private set; }

	public static Dictionary<string, Sprite> ItemIcons;

	public Cart Cart { get; private set; }

	void Start()
	{
		_groupsController = FindObjectOfType<GroupsController>();
		_itemsController = FindObjectOfType<ItemsController>();
		
		CartModel = new CartModel();
		
		ItemIcons = new Dictionary<string, Sprite>();

		XsollaStore.Instance.Token =
			"eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJleHAiOjE5NjIyMzQwNDgsImlzcyI6Imh0dHBzOi8vbG9naW4ueHNvbGxhLmNvbSIsImlhdCI6MTU2MjE0NzY0OCwidXNlcm5hbWUiOiJ4c29sbGEiLCJ4c29sbGFfbG9naW5fYWNjZXNzX2tleSI6IjA2SWF2ZHpDeEVHbm5aMTlpLUc5TmMxVWFfTWFZOXhTR3ZEVEY4OFE3RnMiLCJzdWIiOiJkMzQyZGFkMi05ZDU5LTExZTktYTM4NC00MjAxMGFhODAwM2YiLCJlbWFpbCI6InN1cHBvcnRAeHNvbGxhLmNvbSIsInR5cGUiOiJ4c29sbGFfbG9naW4iLCJ4c29sbGFfbG9naW5fcHJvamVjdF9pZCI6ImU2ZGZhYWM2LTc4YTgtMTFlOS05MjQ0LTQyMDEwYWE4MDAwNCIsInB1Ymxpc2hlcl9pZCI6MTU5MjR9.GCrW42OguZbLZTaoixCZgAeNLGH2xCeJHxl8u8Xn2aI";

		XsollaStore.Instance.CreateNewCart(newCart => { Cart = newCart; }, print);

		XsollaStore.Instance.GetListOfItems(InitStoreUi, print);
	}

	void InitStoreUi(StoreItems items)
	{
		_groupsController.CreateGroups(items);
		_itemsController.CreateItems(items);
	}
}