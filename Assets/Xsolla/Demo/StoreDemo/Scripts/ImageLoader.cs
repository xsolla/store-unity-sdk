using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ImageLoader : MonoBehaviour
{
	private Dictionary<string, Sprite> images;
	private List<string> pendingImages;
	private object locker;

	void Awake()
	{
		locker = new object();
		images = new Dictionary<string, Sprite>();
		pendingImages = new List<string>();
	}

	public void GetImageAsync(string url, Action<string, Sprite> callback)
	{
		if(images.ContainsKey(url)) {
			callback?.Invoke(url, images[url]);
			return;
		}
		lock (locker) {
			if (pendingImages.Contains(url)) {
				StartCoroutine(WaitImage(url, callback));
			} else {
				pendingImages.Add(url);
				StartCoroutine(LoadImage(url, callback));
			}
		}
	}

	IEnumerator LoadImage(string url, Action<string, Sprite> callback)
	{
		Sprite image;
		using (var www = new WWW(url)) {
			yield return www;
			image = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
		}
		lock (locker) {
			images.Add(url, image);
			pendingImages.Remove(url);
		}
		callback?.Invoke(url, image);
	}

	IEnumerator WaitImage(string url, Action<string, Sprite> callback)
	{
		yield return new WaitWhile(() => !images.ContainsKey(url));
		callback?.Invoke(url, images[url]);
	}
}
