using UnityEngine;

namespace Xsolla.Demo
{
	public class RotateCircle : MonoBehaviour
	{
		[SerializeField] private float Speed = 60.0F;

		private RectTransform _circle;

		private void Awake()
		{
			_circle = (RectTransform)transform;
		}

		void Update()
		{
			Vector3 delta = Vector3.forward * Speed * Time.deltaTime;
			_circle.Rotate(delta);
		}
	}
}
