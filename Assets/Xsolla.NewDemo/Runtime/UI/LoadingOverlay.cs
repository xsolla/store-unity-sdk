using UnityEngine;

namespace Xsolla.Demo
{
	public class LoadingOverlay : Screen
	{
		[field: SerializeField] private Transform SpinnerTransform { get; set; }
		[field: SerializeField] private float SpinnerSpeed { get; set; }

		private void Update()
		{
			var delta = Vector3.forward * SpinnerSpeed * Time.deltaTime;
			SpinnerTransform.Rotate(delta);
		}
	}
}