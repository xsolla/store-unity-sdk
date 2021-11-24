using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class BattlePassBuyPremiumSuccessAnimation : MonoBehaviour
    {
		[SerializeField] private Image Highlight;
		[SerializeField] private Image CrystalBig;
		[SerializeField] private float ScaleIncreaseStep;

		private WaitForSeconds _waitTime = new WaitForSeconds(0.0166f);//roughly 60fps

		private void Start()
		{
			StartCoroutine(AnimateScale());
		}

		private IEnumerator AnimateScale()
		{
			Highlight.rectTransform.localScale = Vector2.zero;
			CrystalBig.rectTransform.localScale = Vector2.zero;

			var scaleSize = 0f;

			while (scaleSize < 1.00f)
			{
				yield return _waitTime;

				scaleSize += ScaleIncreaseStep;
				var newScale = new Vector2(scaleSize, scaleSize);

				Highlight.rectTransform.localScale = newScale;
				CrystalBig.rectTransform.localScale = newScale;
			}

			Highlight.rectTransform.localScale = Vector2.one;
			CrystalBig.rectTransform.localScale = Vector2.one;
		}
	}
}
