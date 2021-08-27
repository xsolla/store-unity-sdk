using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.UIBuilder;

namespace Xsolla.Demo
{
	public abstract class BasePageStoreItemsController : MonoBehaviour
	{
		protected const string GROUP_ALL = "ALL";

		[SerializeField] protected WidgetProvider ItemPrefabProvider = new WidgetProvider();
		[SerializeField] protected GroupsController groupsController = default;
		[SerializeField] protected ItemContainer itemsContainer = default;
		[SerializeField] protected GameObject emptyMessage = default;

		private GameObject ItemPrefab => ItemPrefabProvider.GetValue();

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
				var itemGameObject = itemsContainer.AddItem(ItemPrefab);
				InitializeItemUI(itemGameObject, item);
			});
		}

		protected virtual IEnumerator FillGroups()
		{
			yield return new WaitUntil(() => UserCatalog.Instance.IsUpdated);
			yield return new WaitUntil(() => UserInventory.Instance.IsUpdated);

			var groups = GetGroups();

			//Hide BattlePass group if any
			groups.Remove(BattlePassConstants.BATTLEPASS_GROUP);

			groupsController.RemoveAll();
			groupsController.AddGroup(GROUP_ALL);
			groups.ForEach(g => groupsController.AddGroup(g));

			groupsController.SelectDefault();
			
			LayoutRebuilder.ForceRebuildLayoutImmediate(groupsController.transform as RectTransform);
		}

		protected bool CheckHideInAttribute(ItemModel item, HideInFlag flag)
		{
			var attributes = item.Attributes;

			if (attributes == null)
				return false;

			foreach (var attribute in attributes)
			{
				var isHideKey = (attribute.Key == "HideIn");
				var isFlagValue = (attribute.Value == flag.ToString());

				if (isHideKey && isFlagValue)
					return true;
			}

			//else
			return false;
		}

		protected virtual void Initialize() { }

		protected abstract void InitializeItemUI(GameObject item, ItemModel model);
		protected abstract List<ItemModel> GetItemsByGroup(string groupName);
		protected abstract List<string> GetGroups();

		protected enum HideInFlag
		{
			Store,
			Inventory
		}

		protected void ShowEmptyMessage(bool showEmptyMessage)
		{
			emptyMessage.SetActive(showEmptyMessage);
		}

		protected void ShowContent(bool showContent)
		{
			groupsController.gameObject.SetActive(showContent);
			itemsContainer.gameObject.SetActive(showContent);
		}

		protected void UpdateContentVisibility(bool showContent)
		{
			ShowEmptyMessage(!showContent);
			ShowContent(showContent);
		}
	}
}
