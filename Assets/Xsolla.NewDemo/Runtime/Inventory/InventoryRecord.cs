using System;
using UnityEngine;

namespace Xsolla.Demo
{
	[Serializable]
	public class InventoryRecord
	{
		[field: SerializeField] public string Sku { get; set; }
		[field: SerializeField] public int Quantity { get; set; }

		public Sprite IconSprite { get; set; }
	}
}