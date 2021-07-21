using UnityEngine;

namespace Xsolla.Core.Browser
{
	public class RotateCircle : MonoBehaviour
	{
		public float Speed = 60.0F;
		private RectTransform circle;

		private void Start()
		{
			circle = (RectTransform)transform;
		}

		void Update()
		{
			if(circle != null) {
				Vector3 delta = Vector3.forward * Speed * Time.deltaTime;
				circle.Rotate(delta);
			}
		}
	}
}
