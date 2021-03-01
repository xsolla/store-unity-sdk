using System;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	[Serializable]
	public class ThemePropertyProvider
	{
		[SerializeField] private string _id;

		public string Id
		{
			get => _id;
			set => _id = value;
		}
	}
}