using System.Collections;
using UnityEngine;

namespace Xsolla.Core.Popup
{
	public class PopupOverlay : MonoBehaviour
	{
		private void Start()
		{
			StartCoroutine(OverlayCoroutine());
		}

		private void OnDestroy()
		{
			StopAllCoroutines();
		}

		private IEnumerator OverlayCoroutine()
		{
			yield return new WaitForSeconds(0.5F);
			transform.SetAsLastSibling();
			StartCoroutine(OverlayCoroutine());
		}
	}
}
