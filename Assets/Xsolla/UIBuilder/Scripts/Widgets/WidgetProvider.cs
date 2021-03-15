using System;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	[Serializable]
	public class WidgetProvider
	{
		[SerializeField] private string _id;

		public string Id
		{
			get => _id;
			set => _id = value;
		}

		public GameObject GetPrefab()
		{
			return WidgetsLibrary.GetWidgetProperty(Id).Prefab;
		}
	}
}