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

        store.Token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJleHAiOjE5NTgzNTUxODYsImlzcyI6Imh0dHBzOi8vbG9naW4ueHNvbGxhLmNvbSIsImlhdCI6MTU1ODM1NTEyNiwidXNlcm5hbWUiOiJsYWRvcmFAZGlyZWN0bWFpbC50b3AiLCJ4c29sbGFfbG9naW5fYWNjZXNzX2tleSI6Ikw1YzRrdE93a2Fwa3gwNUZldjFKMjVPSmF3QmNiSzNCT1VtUE5sUk1mWjQiLCJzdWIiOiI0YzI0MmRkZS03YWZhLTExZTktOWVjMi00MjAxMGFhODBmZjkiLCJlbWFpbCI6ImxhZG9yYUBkaXJlY3RtYWlsLnRvcCIsInR5cGUiOiJ4c29sbGFfbG9naW4iLCJ4c29sbGFfbG9naW5fcHJvamVjdF9pZCI6IjcyMmNkNDZkLTU1ZTMtMTFlOS05MjQ0LTQyMDEwYWE4MDAwNCIsInB1Ymxpc2hlcl9pZCI6ODgzMDl9.UVddI0mfPiADrE-Iv9HC3Po2nfO0f6q7L_uMXjyQ050";

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
