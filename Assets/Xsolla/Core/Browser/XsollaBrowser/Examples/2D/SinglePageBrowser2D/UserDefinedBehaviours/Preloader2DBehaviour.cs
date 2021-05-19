#if (UNITY_EDITOR || UNITY_STANDALONE)
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(XsollaBrowser))]
public class Preloader2DBehaviour : MonoBehaviour
{
	private int lastProgress;
	private object progressLocker;

	private GameObject prefab;
	private GameObject PreloaderObject;
	private XsollaBrowser xsollaBrowser;

	private void Awake()
	{
		progressLocker = new object();
		lastProgress = 0;
		xsollaBrowser = GetComponent<XsollaBrowser>();
		xsollaBrowser.FetchingBrowserEvent += XsollaBrowser_FetchingBrowserEvent;
		StartCoroutine(PreloaderInstantiateCoroutine());
		StartCoroutine(PreloaderDestroyCoroutine());
	}

	private void OnDestroy()
	{
		xsollaBrowser.FetchingBrowserEvent -= XsollaBrowser_FetchingBrowserEvent;
		StopAllCoroutines();
		if (PreloaderObject != null) {
			Destroy(PreloaderObject);
			PreloaderObject = null;
		}
		progressLocker = null;
	}

	public void SetPreloaderPrefab(GameObject go)
	{
		prefab = go;
	}

	private void XsollaBrowser_FetchingBrowserEvent(int progress)
	{
		lock (progressLocker) {
			if (lastProgress >= progress) return;
			var message = string.Format("Update[%]: {lastProgress} => {progress}", lastProgress, progress);
			Debug.Log(message);
			lastProgress = progress;
			StartCoroutine(PreloaderCoroutine(progress));
		}
	}

	IEnumerator PreloaderCoroutine(int progress)
	{
		yield return new WaitForEndOfFrame();
		if (PreloaderObject == null) yield break;
		if (progress > 99)
			progress = 100;
		PreloaderObject.GetComponent<PreloaderScript>().SetPercent((uint)progress);
	}

	IEnumerator PreloaderInstantiateCoroutine()
	{
		yield return new WaitWhile(() => prefab == null);
		PreloaderObject = Instantiate(prefab, transform);
	}

	IEnumerator PreloaderDestroyCoroutine()
	{
		yield return new WaitWhile(() => xsollaBrowser.FetchingProgress < 100);
		Destroy(this, 0.001F);
	}
}
#endif
