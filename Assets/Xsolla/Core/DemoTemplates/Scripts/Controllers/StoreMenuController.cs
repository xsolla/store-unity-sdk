using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StoreMenuController : MonoBehaviour
{
	private const string ALL_ITEMS_GROUP = "ALL";
	
	[SerializeField] private GameObject itemPrefab;  
	[SerializeField] private GroupsController groupsController;
	[SerializeField] private ItemContainer itemsContainer;
	private IDemoImplementation _demoImplementation;

	private void OnDestroy()
	{
		StopAllCoroutines();
	}

	protected virtual void Start()
	{
		_demoImplementation = DemoController.Instance.GetImplementation();
		groupsController.GroupSelectedEvent += PutItemsToContainer;
		StartCoroutine(CatalogCoroutine());
	}

	private void PutItemsToContainer(string groupName)
	{
		var items = (groupName.Equals(ALL_ITEMS_GROUP))
			? UserCatalog.Instance.AllItems.Where(i => !i.IsVirtualCurrency()).ToList()
			: UserCatalog.Instance.AllItems.Where(i => 
				_demoImplementation.GetCatalogGroupsByItem(i).Contains(groupName)).ToList();
		
		itemsContainer.Clear();
		items.ForEach(i =>
		{
			var go = itemsContainer.AddItem(itemPrefab);
			go.GetComponent<ItemUI>().Initialize(i, _demoImplementation);
		}); 
	}

	private IEnumerator CatalogCoroutine()
	{
		yield return new WaitUntil(() => UserCatalog.Instance.IsUpdated);
		yield return new WaitUntil(() => UserInventory.Instance.IsUpdated);
		CreateAndFillCatalogGroups(UserCatalog.Instance.AllItems);
	}

	private void CreateAndFillCatalogGroups(List<CatalogItemModel> items)
	{
		List<string> groups = new List<string> {ALL_ITEMS_GROUP};
		groupsController.AddGroup(ALL_ITEMS_GROUP);
		
		if (items.Any())
			items.ForEach(i => groups.AddRange(_demoImplementation.GetCatalogGroupsByItem(i)));
		groups = groups.Distinct().ToList();
		groups.Remove(ALL_ITEMS_GROUP);
		groups.ForEach(groupName => groupsController.AddGroup(groupName));
		groupsController.SelectDefault();
	}
}