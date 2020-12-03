using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Xsolla.Demo
{
	public class StoreCurrencyMenuController : MonoBehaviour
	{
		private const string ALL_CURRENCIES_GROUP = "ALL";
	
		[SerializeField] private GameObject itemPrefab = default;
		[SerializeField] private GroupsController groupsController = default;
		[SerializeField] private ItemContainer itemsContainer = default;
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

		private void PutItemsToContainer(string currencyName)
		{
			var items = (currencyName.Equals(ALL_CURRENCIES_GROUP))
				? UserCatalog.Instance.CurrencyPackages
				: UserCatalog.Instance.CurrencyPackages.Where(i =>
				{
					var currency = UserCatalog.Instance.VirtualCurrencies.First(vc => vc.Name.Equals(currencyName));
					return i.CurrencySku.Equals(currency.Sku);
				}).ToList();
		
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
			CreateAndFillCatalogGroups(UserCatalog.Instance.CurrencyPackages);
		}

		private void CreateAndFillCatalogGroups(List<CatalogVirtualCurrencyModel> items)
		{
			var groups = UserCatalog.Instance.VirtualCurrencies
				.Where(vc => items.Any(i => i.CurrencySku.Equals(vc.Sku)))
				.Select(vc => vc.Name).ToList();
		
			groupsController.AddGroup(ALL_CURRENCIES_GROUP);
			groups.ForEach(g => groupsController.AddGroup(g));
		
			groupsController.SelectDefault();
		}
	}
}
