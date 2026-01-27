using UnityEngine;

namespace Xsolla.Demo
{
	public class OwlTargetFollowController : MonoBehaviour
	{
		[field: SerializeField] private OwlTargetFollowState OwlTargetFollowState { get; set; }
		[field: Header("Debug")]
		[field: SerializeField] private GuyMovementState GuyMovementState { get; set; }

		public void Construct(GuyMovementState guyMovementState)
		{
			GuyMovementState = guyMovementState;
		}

		private void Update()
		{
			if (!GuyMovementState)
				return;

			OwlTargetFollowState.TargetPosition = GuyMovementState.transform.position;
			OwlTargetFollowState.TargetVelocity = GuyMovementState.CurrentVelocity;
		}
	}
}