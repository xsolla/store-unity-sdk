using UnityEngine;

namespace Xsolla.Demo
{
	public class FoxAnimator : MonoBehaviour, IPawnModeComponent
	{
		[SerializeField] private Transform Root;

		public void ToggleMode(PawnMode pawnMode)
		{
			Root.rotation = Quaternion.LookRotation(Vector3.back);
		}
	}
}