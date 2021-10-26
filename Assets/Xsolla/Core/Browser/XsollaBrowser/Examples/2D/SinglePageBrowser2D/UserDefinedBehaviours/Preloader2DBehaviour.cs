#if (UNITY_EDITOR || UNITY_STANDALONE)
using UnityEngine;
using System.Collections;

namespace Xsolla.Core.Browser
{
	[RequireComponent(typeof(XsollaBrowser))]
	public class Preloader2DBehaviour : MonoBehaviour
	{
		private int lastProgress;
		private object progressLocker;
		private GameObject preloaderObject;
		private XsollaBrowser xsollaBrowser;

		public GameObject Prefab { get; set; }

		private void Awake()
		{
			progressLocker = new object();
			lastProgress = 0;

			xsollaBrowser = GetComponent<XsollaBrowser>();
			xsollaBrowser.FetchingBrowserEvent += OnBrowserFetchingEvent;

			StartCoroutine(PreloaderInstantiateCoroutine());
			StartCoroutine(PreloaderDestroyCoroutine());
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

			if (progress > 99)
				progress = 100;

			preloaderObject.GetComponent<PreloaderScript>().SetPercent((uint) progress);
		}

		private IEnumerator PreloaderInstantiateCoroutine()
		{
			yield return new WaitWhile(() => Prefab == null);
			preloaderObject = Instantiate(Prefab, transform);
		}

		private IEnumerator PreloaderDestroyCoroutine()
		{
			yield return new WaitWhile(() => xsollaBrowser.FetchingProgress < 100);
			Destroy(this, 0.001f);
		}
	}
}
#endif