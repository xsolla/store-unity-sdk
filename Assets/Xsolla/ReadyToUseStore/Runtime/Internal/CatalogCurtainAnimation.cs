using UnityEngine;

namespace Xsolla.ReadyToUseStore
{
	internal class CatalogCurtainAnimation : MonoBehaviour
	{
		[SerializeField] private Transform RotateTransform;
		[SerializeField] private float RotateSpeed = 1f;

		private void Update()
		{
			var delta = Vector3.forward * RotateSpeed * Time.deltaTime;
			RotateTransform.Rotate(delta);
		}
	}
}