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

    public Cart Cart { get; private set; }

    private void Start()
    {
        store = XsollaStore.Instance;

        store.Token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJleHAiOjE5NTgzNTUxODYsImlzcyI6Imh0dHBzOi8vbG9naW4ueHNvbGxhLmNvbSIsImlhdCI6MTU1ODM1NTEyNiwidXNlcm5hbWUiOiJsYWRvcmFAZGlyZWN0bWFpbC50b3AiLCJ4c29sbGFfbG9naW5fYWNjZXNzX2tleSI6Ikw1YzRrdE93a2Fwa3gwNUZldjFKMjVPSmF3QmNiSzNCT1VtUE5sUk1mWjQiLCJzdWIiOiI0YzI0MmRkZS03YWZhLTExZTktOWVjMi00MjAxMGFhODBmZjkiLCJlbWFpbCI6ImxhZG9yYUBkaXJlY3RtYWlsLnRvcCIsInR5cGUiOiJ4c29sbGFfbG9naW4iLCJ4c29sbGFfbG9naW5fcHJvamVjdF9pZCI6IjcyMmNkNDZkLTU1ZTMtMTFlOS05MjQ0LTQyMDEwYWE4MDAwNCIsInB1Ymxpc2hlcl9pZCI6ODgzMDl9.UVddI0mfPiADrE-Iv9HC3Po2nfO0f6q7L_uMXjyQ050";

        SubscribeToEvents();

        CreateCart();
        
        store.GetListOfItems((items) =>
        {
	        CreateGroups(items);
	        CreateItems(items);
        }, error =>
        {
	        Debug.Log(error.ToString());
        });
    }

    void CreateCart()
    {
	    store.CreateNewCart(newCart =>
	    {
		    Cart = newCart;
		    print(Cart.id);
	    }, error =>
	    {
		    print(error.ToString());
	    });
    }
    
    public void ClearCart()
    {
	    store.ClearCart(Cart, () =>
	    {
		    print("Heeeey!");
	    }, error =>
	    {
		    print(error.ToString());
	    });
    }

    private void SubscribeToEvents()
    {
        _groupsController.OnGroupClick += (id) => { _itemsController.ActivateContainer(id); };
    }

    private void CreateItems(StoreItems items)
    {
        foreach (var item in items.items)
        {
            _itemsController.AddItem(item);
        }
        
        _itemsController.CreateCart();
    }

    private void CreateGroups(StoreItems items)
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
}
