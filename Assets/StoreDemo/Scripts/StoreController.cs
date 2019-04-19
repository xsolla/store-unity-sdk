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

        SubscribeToEvents();

        if (store.Token == "")
        {
            var user = JsonUtility.FromJson<XsollaStore.TokenInformation>("{\"user\":{\"id\":{\"value\":\"user_2\"},\"public_id\":{\"value\":\"user_2\"},\"name\":{\"value\":\"John Smith\"},\"email\":{\"value\":\"john.smith@mail.com\"},\"country\":{\"value\":\"US\",\"allow_modify\":true}},\"settings\":{\"project_id\":42638,\"mode\":\"sandbox\",\"currency\":\"USD\",\"language\":\"en\",\"ui\":{\"size\":\"medium\",\"desktop\":{\"virtual_item_list\":{\"layout\":\"list\",\"button_with_price\":true}},\"components\":{\"virtual_currency\":{\"custom_amount\":true}}}},\"purchase\":{\"virtual_currency\":{\"quantity\":100},\"virtual_items\":{\"items\":[{\"sku\":\"SKU01\",\"amount\":1}]}}}");
            //generate new token
            store.GetToken(user);
        }
        else
            Debug.Log("Saved token: "+store.Token);

        store.GetListOfGroups();
        store.GetListOfItems();
    }

    private void SubscribeToEvents()
    {
        //Debug Log messages
        store.OnSuccesfullGetToken += (str) => { Debug.Log("New token: " + str); };
        store.OnCantParseToken += (str) => { Debug.Log("Cant Parse " + str); };
        store.OnInvalidProjectSettings += (error) => { Debug.Log("OnInvalidProjectSettings " + error.extended_message); };
        store.OnInvalidData += (error) => { Debug.Log("OnInvalidData " + error.extended_message); };
        store.OnIdentifiedError += (error) => { Debug.Log("OnIdentifiedError " + error.extended_message); };
        store.OnNetworkError += () => { Debug.Log("Network Error"); };


        store.OnSuccessGetListOfItems += (items) =>
        {
            foreach (var item in items.storeItems)
            {
                //Debug.Log(item.localized_name);
                store.GetItemInformation(item.id);
            }
        };
        store.OnSuccessGetListOfGroups += (items) =>
        {
            foreach (var item in items.groupItems)
            {
                //Debug.Log(item.localized_name);
                store.GetGroupInformation(item.id);
            }
        };
        store.OnSuccessGetGroupInformation += (item) => { _groupsController.AddGroup(item); };
        store.OnSuccessGetItemInformation += (item) => { _itemsController.AddItem(item); };

        //Local events
        _groupsController.OnGroupClick += (id) => { _itemsController.ActivateContainer(id); };
    }
}
