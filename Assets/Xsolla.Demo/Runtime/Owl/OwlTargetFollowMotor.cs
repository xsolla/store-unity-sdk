using UnityEngine;

namespace Xsolla.Demo
{
	public class OwlTargetFollowMotor : MonoBehaviour
	{
		[field: SerializeField] private OwlTargetFollowState OwlTargetFollowState { get; set; }
		[field: SerializeField] private Transform PawnRoot { get; set; }
		[field: SerializeField] private Vector3 TargetOffset { get; set; }
		[field: SerializeField] private float PositionLerpFactor { get; set; } = 1f;
		[field: SerializeField] private float RotationLerpFactor { get; set; } = 10f;

		[field: Header("Debug")]
		[field: SerializeField] private Vector3 TargetPosition { get; set; }

		private TimeService TimeService => ServiceLocator.Resolve<TimeService>();

		private void Update()
		{
			var deltaTime = TimeService.DeltaTime;
			UpdatePosition(PositionLerpFactor * deltaTime);
			UpdateRotation(RotationLerpFactor * deltaTime);
		}

		public void AlignToTargetImmediately()
		{
			UpdatePosition(1f);
			UpdateRotation(1f);
		}

		private void UpdatePosition(float lerpFactor)
		{
			var targetOffset = TargetOffset;
			targetOffset.x *= Mathf.Sign(OwlTargetFollowState.TargetVelocity.x);
			TargetPosition = OwlTargetFollowState.TargetPosition + targetOffset;

			PawnRoot.position = Vector3.Lerp(PawnRoot.position, TargetPosition, lerpFactor);
		}

		private void UpdateRotation(float lerpFactor)
		{
			var toTarget = TargetPosition - PawnRoot.position;

			var lookDirection = Vector3.back;
			if (toTarget.sqrMagnitude > 0.1f)
			{
				lookDirection = toTarget.x > 0
					? Vector3.right
					: Vector3.left;
			}

			var targetRotation = Quaternion.LookRotation(lookDirection);
			PawnRoot.rotation = Quaternion.Lerp(PawnRoot.rotation, targetRotation, lerpFactor);
		}
	}
}