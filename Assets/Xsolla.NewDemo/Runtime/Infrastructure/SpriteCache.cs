using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public static class SpriteCache
	{
		private static readonly Dictionary<string, Sprite> Sprites = new();

		public static Sprite GetSpriteFromUrl(string url)
		{
			return Sprites.GetValueOrDefault(url);
		}

		public static IEnumerator LoadSprite(string url, Action<Sprite> action)
		{
			var done = false;

			ImageLoader.LoadSprite(
				url,
				sprite => {
					done = true;
					Sprites[url] = sprite;
					action?.Invoke(sprite);
				});

			yield return new WaitUntil(() => done);
		}
	}
}