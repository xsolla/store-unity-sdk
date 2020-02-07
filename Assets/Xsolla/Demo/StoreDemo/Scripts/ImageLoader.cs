using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Collections.Concurrent;
using System.Linq;

public class ImageLoader : MonoBehaviour
{
	private Dictionary<string, Sprite> images;
	private List<string> pendingImages;

	void Awake()
	{
		images = new Dictionary<string, Sprite>();
		pendingImages = new List<string>();
	}

	public void GetImageAsync(string url, Action<string, Sprite> callback)
	{
		if(images.ContainsKey(url)) {
			callback?.Invoke(url, images[url]);
			return;
		}
		if (!pendingImages.Contains(url)) {
			pendingImages.Add(url);
			StartCoroutine(LoadImage(url, callback));
		}
		StartCoroutine(WaitImage(url, callback));
	}

	IEnumerator LoadImage(string url, Action<string, Sprite> callback)
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
