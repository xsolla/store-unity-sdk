using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Xsolla.Demo
{
	[Serializable]
	public class InventoryStorage
	{
		[SerializeField] private List<InventoryRecord> Items = new();

		public IEnumerable<InventoryRecord> GetAllItems()
		{
			return Items;
		}

		public InventoryRecord GetItem(string sku)
		{
			var item = Items.FirstOrDefault(x => x.Sku == sku);
			if (item != null)
				return item;

			item = new InventoryRecord {
				Sku = sku,
				Quantity = 0
			};

			Items.Add(item);
			return item;
		}

		public void UpdateQuantity(string sku, int quantity)
		{
			GetItem(sku).Quantity = quantity;
		}

		public void UpdateIconSprite(string sku, Sprite sprite)
		{
			GetItem(sku).IconSprite = sprite;
		}

		public void Clear()
		{
			Items.Clear();
		}
	}
}