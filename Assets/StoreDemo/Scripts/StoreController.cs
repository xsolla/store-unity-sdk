using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xsolla;

public class StoreController : MonoBehaviour
{
    [SerializeField]
    private GroupsController _groupsController;
    [SerializeField]
    private ItemsController _itemsController;

    private XsollaStore store;

    private void Start()
    {
        store = XsollaStore.Instance;

        store.Token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJleHAiOjE1NTgxMDIyNzUsImlzcyI6Imh0dHBzOlwvXC9sb2dpbi54c29sbGEuY29tIiwiaWF0IjoxNTU4MTAyMjE1LCJ1c2VybmFtZSI6ImFsIiwieHNvbGxhX2xvZ2luX2FjY2Vzc19rZXkiOiI5OXBuQXdVRnk2YjUwenktUFlwU2huZ3pZUmFIbHNUU3Y2SktWRmNicnpjIiwic3ViIjoiZTA1NDhhZmUtNTVmYi0xMWU5LWFkZWUtNDIwMTBhYTgwMDIxIiwiZW1haWwiOiJhLmtpc2xheWFAdmlyb25pdC5jby51ayIsInR5cGUiOiJ4c29sbGFfbG9naW4iLCJ4c29sbGFfbG9naW5fcHJvamVjdF9pZCI6IjcyMmNkNDZkLTU1ZTMtMTFlOS05MjQ0LTQyMDEwYWE4MDAwNCIsInB1Ymxpc2hlcl9pZCI6ODgzMDl9.LKjKKL8ElZiPfWI-oXX1ECyKxsDwGDN4LNskb0X878Y";

        SubscribeToEvents();
        
        store.GetListOfItems();
    }

    private void SubscribeToEvents()
    {
        //Debug Log messages
        store.OnInvalidProjectSettings += (error) => { Debug.Log("OnInvalidProjectSettings " + error.extended_message); };
        store.OnInvalidData += (error) => { Debug.Log("OnInvalidData " + error.extended_message); };
        store.OnIdentifiedError += (error) => { Debug.Log("OnIdentifiedError " + error.extended_message); };
        store.OnNetworkError += () => { Debug.Log("Network Error"); };


        store.OnSuccessGetListOfItems += (items) =>
        {
            CreateGroups(items);
            CreateItems(items);
        };

        //Local events
        _groupsController.OnGroupClick += (id) => { _itemsController.ActivateContainer(id); };
    }

    private void CreateItems(XsollaStore.StoreItems items)
    {
        foreach (var item in items.storeItems)
        {
            _itemsController.AddItem(item);
        }
    }

    private void CreateGroups(XsollaStore.StoreItems items)
    {
        List<string> addedGroups = new List<string>();
        foreach (var item in items.storeItems)
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
    }
}
