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

		protected override void OnDestroy()
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
			yield return WebRequestHelper.Instance.ImageRequestCoroutine(url, sprite =>
			{
				images.Add(url, sprite);
				pendingImages.Remove(url);
			});
		}

		IEnumerator WaitImage(string url, Action<string, Sprite> callback)
		{
			yield return new WaitUntil(() => images.ContainsKey(url));
			callback?.Invoke(url, images[url]);
		}
	}
}
