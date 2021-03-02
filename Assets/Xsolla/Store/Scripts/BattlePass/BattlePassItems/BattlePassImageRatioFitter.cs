using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class BattlePassImageRatioFitter : MonoBehaviour
    {
		[SerializeField] private AspectRatioFitter ItemImageRatioFitter = default;

		public void SetAspectRatio(ItemImageContainer itemImageContainer)
		{
			StartCoroutine(SetAspectRatioOnImageArrival(itemImageContainer));
		}

		private IEnumerator SetAspectRatioOnImageArrival(ItemImageContainer imageContainer)
		{
			yield return new WaitWhile(() => imageContainer.Image == null);
			FitImageAspectRatio(ItemImageRatioFitter, imageContainer.Image);
		}

		private void FitImageAspectRatio(AspectRatioFitter aspectRatioFitter, Sprite image)
		{
			var width = image.rect.width;
			var height = image.rect.height;
			var targetAspectRatio = width / height;

			if (targetAspectRatio > 1.0f)
				aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
			else
				aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;

			aspectRatioFitter.aspectRatio = targetAspectRatio;
		}
	}
}
