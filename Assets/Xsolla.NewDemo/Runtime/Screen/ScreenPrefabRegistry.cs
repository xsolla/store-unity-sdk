using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Demo
{
	public class ScreenPrefabRegistry : ScriptableObject
	{
		[field: SerializeField] private List<Screen> Screens;

		public TScreen GetPrefab<TScreen>() where TScreen : Screen
		{
			foreach (var screen in Screens)
			{
				if (screen is TScreen prefab)
					return prefab;
			}

			throw new Exception($"Screen of type {typeof(TScreen)} is not registered in ScreenRegistry");
		}
	}
}