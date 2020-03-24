using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Xsolla.Core
{
	public class ImageLoader : MonoSingleton<ImageLoader>
	{
		private Dictionary<string, Sprite> images;
		private List<string> pendingImages;

		public override void Init()
		{
			base.Init();
			images = new Dictionary<string, Sprite>();
			pendingImages = new List<string>();
		}

		private void OnDestroy()
		{
			StopAllCoroutines();
			pendingImages.Clear();
			images.Clear();
		}

		public void GetImageAsync(string url, Action<string, Sprite> callback)
		{
			if (images.ContainsKey(url)) {
				callback?.Invoke(url, images[url]);
				return;
			}
			if (!pendingImages.Contains(url)) {
				pendingImages.Add(url);
				StartCoroutine(LoadImage(url));
			}
			if(callback != null) {
				StartCoroutine(WaitImage(url, callback));
			}
		}

		IEnumerator LoadImage(string url)
		{
			Sprite image;
			using (var www = new WWW(url)) {
				yield return www;
				image = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
			}
			images.Add(url, image);
			pendingImages.Remove(url);
		}

		IEnumerator WaitImage(string url, Action<string, Sprite> callback)
		{
			yield return new WaitUntil(() => images.ContainsKey(url));
			callback?.Invoke(url, images[url]);
		}
	}
}
