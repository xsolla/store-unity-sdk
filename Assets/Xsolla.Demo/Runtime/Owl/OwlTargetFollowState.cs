using UnityEngine;

namespace Xsolla.Demo
{
	public class OwlTargetFollowState : MonoBehaviour
	{
		[field: SerializeField] public Vector3 TargetPosition { get; set; }
		[field: SerializeField] public Vector3 TargetVelocity { get; set; }
	}
}