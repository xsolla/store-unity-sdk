using UnityEngine;

namespace Xsolla.Demo
{
	internal class GuyAnimator : MonoBehaviour, IPawnModeComponent
	{
		[SerializeField] private GuyMovementState GuyMovementState;
		[SerializeField] private Animator Animator;
		[SerializeField] private Transform Root;

		private static readonly int IsJumpingKey = Animator.StringToHash("IsJumping");
		private static readonly int MovementSpeedKey = Animator.StringToHash("MovementSpeed");

		private void Start()
		{
			Root.rotation = Quaternion.LookRotation(Vector3.back);

			GuyMovementState.JumpBegan += OnJumpBegan;
			GuyMovementState.JumpEnded += OnJumpEnded;
		}

		private void OnDestroy()
		{
			GuyMovementState.JumpBegan -= OnJumpBegan;
			GuyMovementState.JumpEnded -= OnJumpEnded;
		}

		private void Update()
		{
			var inputHorizontal = GuyMovementState.InputHorizontal;
			Animator.SetFloat(MovementSpeedKey, Mathf.Abs(inputHorizontal));
		}

		public void ToggleMode(PawnMode pawnMode)
		{
			ToggleAnimatorLayer(pawnMode == PawnMode.Gameplay);
		}

		private void OnJumpBegan()
			=> Animator.SetBool(IsJumpingKey, true);

		private void OnJumpEnded()
			=> Animator.SetBool(IsJumpingKey, false);

		private void ToggleAnimatorLayer(bool isPlayMode)
			=> Animator.SetLayerWeight(1, isPlayMode ? 0 : 1);
	}
}