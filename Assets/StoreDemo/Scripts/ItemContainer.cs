using UnityEngine;

public class ItemContainer : MonoBehaviour
{
	[SerializeField]
	GameObject _item_Prefab;

	[SerializeField]
	Transform _item_Parent;

	public void AddItem(Xsolla.StoreItem itemInformation)
	{
		GameObject newItem = Instantiate(_item_Prefab, _item_Parent);
		newItem.GetComponent<ItemUI>().Initialize(itemInformation);
	}
}