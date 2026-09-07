using UnityEngine;

namespace Xsolla.Demo
{
	internal class GuyPlayerController : MonoBehaviour, IPawnModeComponent
	{
		[SerializeField] private GuyMovementState GuyMovementState;
		[SerializeField] private GroundDetector GroundDetector;
		[SerializeField] private float GroundCheckDistance = 0.1f;
		[SerializeField] private float MoveSpeed = 5f;
		[SerializeField] private float JumpHeight = 2f;

		private TimeService TimeService => ServiceLocator.Resolve<TimeService>();

		private Vector3 CurrentVelocity;
		private bool IsJumping;

		private void Update()
		{
			var deltaTime = TimeService.DeltaTime;
			UpdateVelocity(deltaTime);
		}

		public void ToggleMode(PawnMode pawnMode)
		{
			enabled = pawnMode == PawnMode.Gameplay;
		}

		private void UpdateVelocity(float deltaTime)
		{
			var isGrounded = GroundDetector.CurrentDistance <= GroundCheckDistance;
			if (isGrounded)
			{
				if (IsJumping && CurrentVelocity.y < 0)
				{
					IsJumping = false;
					GuyMovementState.OnJumpEnded();
				}

				if (CurrentVelocity.y < 0)
				{
					CurrentVelocity.y = -2f;
				}

				if (!IsJumping && Input.GetKeyDown(KeyCode.Space))
				{
					IsJumping = true;
					CurrentVelocity.y = Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y);
					GuyMovementState.OnJumpBegan();
				}
			}

			var inputHorizontal = Input.GetAxis("Horizontal");
			CurrentVelocity.x = inputHorizontal * MoveSpeed;
			CurrentVelocity.y += Physics.gravity.y * deltaTime;

			GuyMovementState.InputHorizontal = inputHorizontal;
			GuyMovementState.CurrentVelocity = CurrentVelocity;
		}
	}
}