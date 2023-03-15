#if (UNITY_EDITOR || UNITY_STANDALONE)
using UnityEngine;
using System.Collections;

namespace Xsolla.Core.Browser
{
	public class Preloader2DBehaviour : MonoBehaviour
	{
		private int lastProgress;
		private object progressLocker;
		private GameObject preloaderObject;
		private XsollaBrowser xsollaBrowser;
		private GameObject prefab;

		private void Awake()
		{
			progressLocker = new object();
			lastProgress = 0;

			xsollaBrowser = GetComponent<XsollaBrowser>();
			xsollaBrowser.FetchingBrowserEvent += OnBrowserFetchingEvent;

			StartCoroutine(PreloaderInstantiateCoroutine());
		}

		private void OnDestroy()
		{
			xsollaBrowser.FetchingBrowserEvent -= OnBrowserFetchingEvent;
			StopAllCoroutines();

			if (preloaderObject)
			{
				Destroy(preloaderObject);
				preloaderObject = null;
			}

			progressLocker = null;
		}

		public void SetPrefab(GameObject obj)
		{
			prefab = obj;
		}

		private void OnBrowserFetchingEvent(int progress)
		{
			lock (progressLocker)
			{
				if (lastProgress >= progress)
					return;

				Debug.Log($"Update[%]: {lastProgress} => {progress}");
				lastProgress = progress;

				StartCoroutine(PreloaderCoroutine(progress));
			}
		}

		private IEnumerator PreloaderCoroutine(int progress)
		{
			yield return new WaitForEndOfFrame();

			if (preloaderObject == null)
				yield break;

			if (progress < 99)
				preloaderObject.GetComponent<PreloaderScript>().SetPercent((uint) progress);
			else
				preloaderObject.GetComponent<PreloaderScript>().SetText(string.Empty);
		}

		private IEnumerator PreloaderInstantiateCoroutine()
		{
			yield return new WaitWhile(() => prefab == null);
			preloaderObject = Instantiate(prefab, transform);
		}
	}
}
#endif