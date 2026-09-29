using UnityEngine;

namespace Xsolla.Demo
{
	internal class GuyMovementMotor : MonoBehaviour
	{
		[SerializeField] private Transform PawnRoot;
		[SerializeField] private GuyMovementState GuyMovementState;
		[SerializeField] private CharacterController CharacterController;
		[SerializeField] private GroundDetector GroundDetector;
		[SerializeField] private float RotationLerpFactor = 10f;

		private TimeService TimeService => ServiceLocator.Resolve<TimeService>();

		private float MoveDirection;
		private Vector3 LastLookDirection;

		private void Update()
		{
			var deltaTime = TimeService.DeltaTime;
			UpdatePosition(deltaTime);
			UpdateRotation(deltaTime);
		}

		private void UpdatePosition(float deltaTime)
		{
			var velocity = GuyMovementState.CurrentVelocity;
			CharacterController.Move(velocity * deltaTime);
		}

		private void UpdateRotation(float deltaTime)
		{
			var inputHorizontal = GuyMovementState.InputHorizontal;

			if (Mathf.Abs(inputHorizontal) > 0.1f)
				MoveDirection = Mathf.Sign(inputHorizontal);

			var lookDirection = MoveDirection * Vector3.right;

			if (Mathf.Abs(inputHorizontal) < 0.1f && GroundDetector.CurrentDistance < 0.1f)
				lookDirection = Vector3.back;

			if (lookDirection.sqrMagnitude > 0.0001f)
				LastLookDirection = lookDirection;

			var targetRotation = Quaternion.LookRotation(LastLookDirection);
			PawnRoot.rotation = Quaternion.Lerp(PawnRoot.rotation, targetRotation, RotationLerpFactor * deltaTime);
		}
	}
}