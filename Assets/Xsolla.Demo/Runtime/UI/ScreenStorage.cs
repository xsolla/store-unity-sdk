using System;
using System.Collections.Generic;

namespace Xsolla.Demo
{
	public class ScreenStorage
	{
		private readonly Dictionary<Type, Screen> Screens = new();

		public IEnumerable<Screen> GetAll()
			=> Screens.Values;

		public TScreen Get<TScreen>() where TScreen : Screen
		{
			var type = typeof(TScreen);
			if (Screens.TryGetValue(type, out var screen))
				return (TScreen) screen;

			return null;
		}

		public void Add<TScreen>(TScreen screen) where TScreen : Screen
		{
			var type = typeof(TScreen);
			if (!Screens.TryAdd(type, screen))
				throw new Exception($"Screen of type {type} is already added to storage");
		}

		public void Remove(Screen screen)
		{
			var type = screen.GetType();
			Screens.Remove(type);
		}
	}
}