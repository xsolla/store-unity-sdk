using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Xsolla.Core;

namespace Xsolla.Demo
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
				LoadImage(url);
				StartCoroutine(WaitImage(url, callback));
			}
		}

		private void LoadImage(string url)
		{
			WebRequestHelper.Instance.ImageRequest(url,
				onComplete: sprite => images[url] = sprite,
				onError: error => XDebug.LogError(error.errorMessage));
		}

		private IEnumerator WaitImage(string url, Action<string, Sprite> callback)
		{
			yield return new WaitWhile(() => images[url] == null);
			callback?.Invoke(url, images[url]);
		}
	}
}
