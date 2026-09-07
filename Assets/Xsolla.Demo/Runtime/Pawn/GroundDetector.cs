using UnityEngine;

namespace Xsolla.Demo
{
	internal class GroundDetector : MonoBehaviour
	{
		[SerializeField] private Transform CastPivot;
		[SerializeField] private float CastDistance;
		[SerializeField] private LayerMask CastLayer;
		[Header("Debug")]
		[SerializeField] private float _currentDistance;
		[SerializeField] private Color CastSuccessDebugColor = Color.green;
		[SerializeField] private Color CastFailDebugColor = Color.red;

		private const float CastOffset = 0.1f;

		public float CurrentDistance
		{
			get => _currentDistance;
			private set => _currentDistance = value;
		}

		private void Update()
		{
			var ray = new Ray(CastPivot.position + Vector3.up * CastOffset, Vector3.down);
			var totalCastDistance = CastDistance + CastOffset;

			if (Physics.Raycast(ray, out var hit, totalCastDistance, CastLayer.value))
			{
				CurrentDistance = hit.distance - CastOffset;
				Debug.DrawLine(ray.origin, hit.point, CastSuccessDebugColor);
			}
			else
			{
				CurrentDistance = CastDistance;
				Debug.DrawRay(ray.origin, ray.direction * totalCastDistance, CastFailDebugColor);
			}
		}
	}
}