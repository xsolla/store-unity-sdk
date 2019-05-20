using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    [SerializeField]
    private GameObject _item_Prefab;
    [SerializeField]
    private Transform _item_Parent;

    public void AddItem(Xsolla.XsollaStore.StoreItem itemInformation)
    {
        GameObject newItem = Instantiate(_item_Prefab, _item_Parent);
        newItem.GetComponent<ItemUI>().Initialize(itemInformation);
    }
}
