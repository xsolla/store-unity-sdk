using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Demo
{
	public class CatalogStorage
	{
		private readonly Dictionary<string, CatalogRecord> Items = new();

		public CatalogRecord GetItem(string sku)
		{
			return Items.GetValueOrDefault(sku);
		}

		public void AddItem(CatalogRecord record)
		{
			Items[record.Sku] = record;
		}

		public void UpdateIcon(string sku, Sprite sprite)
		{
			GetItem(sku).UpdateIconSprite(sprite);
		}

		public void Clear()
		{
			Items.Clear();
		}
	}
}