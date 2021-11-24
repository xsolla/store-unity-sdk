using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper : MonoSingleton<WebRequestHelper>
	{
		public void ImageRequest(string url, Action<Texture2D> onComplete = null, Action<Error> onError = null)
		{
			StartCoroutine(ImageRequestCoroutine(url, onComplete, onError));
		}

		public void ImageRequest(string url, Action<Sprite> onComplete = null, Action<Error> onError = null)
		{
			StartCoroutine(ImageRequestCoroutine(url, onComplete, onError));
		}

		private IEnumerator ImageRequestCoroutine(string url, Action<Texture2D> onComplete = null, Action<Error> onError = null)
		{
			var webRequest = UnityWebRequestTexture.GetTexture(url);
			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError));
		}
		
		private IEnumerator ImageRequestCoroutine(string url, Action<Sprite> onComplete = null, Action<Error> onError = null)
		{
			yield return ImageRequestCoroutine(url, texture2D =>
			{
				Rect spriteRect = new Rect(0, 0, texture2D.width, texture2D.height);
				Vector2 pivot = new Vector2(0.5f, 0.5f);
				Sprite sprite = Sprite.Create(texture2D, spriteRect, pivot);
				if (onComplete != null)
					onComplete.Invoke(sprite);
			}, onError);
		}
	}
}

