using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Xsolla.Core
{
	public class ImageLoader : MonoSingleton<ImageLoader>
	{
		private Dictionary<string, Sprite> images;

		public override void Init()
		{
			base.Init();
			images = new Dictionary<string, Sprite>();
		}

		protected override void OnDestroy()
		{
			StopAllCoroutines();
			images?.Clear();
		}

		public void AddImage(string url, Sprite image)
		{
			images[url] = image;
		}

		public void GetImageAsync(string url, Action<string, Sprite> callback)
		{
			if (images.ContainsKey(url) && images[url] != null)
			{
				callback?.Invoke(url, images[url]);
			}
			else if (images.ContainsKey(url)/*&& images[url] == null*/)
			{
				StartCoroutine(WaitImage(url, callback));
			}
			else/*if (!images.ContainsKey(url))*/
			{
				images[url] = null;
				StartCoroutine(LoadImage(url));
				StartCoroutine(WaitImage(url, callback));
			}
		}

		IEnumerator LoadImage(string url)
		{
			yield return WebRequestHelper.Instance.ImageRequestCoroutine(url, 
				onComplete: sprite => images[url] = sprite,
				onError: error => Debug.LogError(error.errorMessage));
		}

		IEnumerator WaitImage(string url, Action<string, Sprite> callback)
		{
			yield return new WaitWhile(() => images[url] == null);
			callback?.Invoke(url, images[url]);
		}
	}
}
