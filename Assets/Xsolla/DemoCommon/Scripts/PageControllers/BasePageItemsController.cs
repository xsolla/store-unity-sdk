using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Demo
{
	public abstract class BasePageItemsController : MonoBehaviour
	{
		protected const string GROUP_ALL = "ALL";

		[SerializeField] protected GameObject itemPrefab = default;
		[SerializeField] protected GroupsController groupsController = default;
		[SerializeField] protected ItemContainer itemsContainer = default;

		private void Start()
		{
			Initialize();

			groupsController.GroupSelectedEvent += ShowGroupItems;
			StartCoroutine(FillGroups());
		}


		protected void ShowGroupItems(string groupName)
		{
			var items = GetItemsByGroup(groupName);

			itemsContainer.Clear();
			items.ForEach(item =>
			{
				var itemGameObject = itemsContainer.AddItem(itemPrefab);
				InitializeItemUI(itemGameObject, item);
			});
		}

		protected IEnumerator FillGroups()
		{
			yield return new WaitUntil(() => UserCatalog.Instance.IsUpdated);
			yield return new WaitUntil(() => UserInventory.Instance.IsUpdated);

			var groups = GetGroups();

			groupsController.RemoveAll();
			groupsController.AddGroup(GROUP_ALL);
			groups.ForEach(g => groupsController.AddGroup(g));

			groupsController.SelectDefault();
		}

		protected abstract void Initialize();
		protected abstract void InitializeItemUI(GameObject item, ItemModel model);
		protected abstract List<ItemModel> GetItemsByGroup(string groupName);
		protected abstract List<string> GetGroups();
	}
}
