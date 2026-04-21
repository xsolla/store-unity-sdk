using System.Linq;
using Cinemachine;
using UnityEngine;

namespace Xsolla.Demo
{
	public class GuyCameraController : MonoBehaviour, IPawnModeComponent
	{
		[SerializeField] private Transform Root;
		[SerializeField] private CinemachineVirtualCamera VirtualCamera;

		private void Awake()
		{
			VirtualCamera = FindObjectsByType<VirtualCamera>(FindObjectsInactive.Include, FindObjectsSortMode.None)
				.First(x => x.PawnMode == PawnMode.Gameplay)
				.GetComponent<CinemachineVirtualCamera>();
		}

		private void Update()
		{
			var position = transform.position;
			position.y = 0;
			position.z = 0;
			Root.position = position;
		}

		public void ToggleMode(PawnMode pawnMode)
		{
			VirtualCamera.Follow = Root;
		}
	}
}