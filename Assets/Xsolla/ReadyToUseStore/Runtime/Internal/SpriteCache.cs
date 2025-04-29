using System;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.ReadyToUseStore
{
	internal static class SpriteCache
	{
		private static readonly Dictionary<string, SpriteCacheHandler> Handlers = new();

		public static void Get(string url, Action<Sprite> callback)
		{
			var handler = Handlers.GetValueOrDefault(url);
			if (handler == null)
			{
				handler = new SpriteCacheHandler();
				Handlers[url] = handler;
			}

			if (handler.Sprite)
			{
				callback?.Invoke(handler.Sprite);
				return;
			}

			if (callback != null)
				handler.Callbacks.Add(callback);

			ImageLoader.LoadSprite(
				url,
				sprite => OnSpriteLoaded(handler, sprite));
		}

		private static void OnSpriteLoaded(SpriteCacheHandler handler, Sprite sprite)
		{
			handler.Sprite = sprite;

			foreach (var callback in handler.Callbacks)
			{
				callback?.Invoke(sprite);
			}

			handler.Callbacks.Clear();
		}
	}
}