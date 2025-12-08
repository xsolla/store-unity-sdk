using System.Linq;
using UnityEngine;

namespace Xsolla.Demo
{
	public class CameraService
	{
		public void ToggleCamera(PawnMode pawnMode)
		{
			if (pawnMode == PawnMode.Gameplay)
			{
				GetCamera(PawnMode.Gameplay).SetActive(true);
				GetCamera(PawnMode.Onboarding).SetActive(false);
			}
			else
			{
				GetCamera(PawnMode.Gameplay).SetActive(false);
				GetCamera(PawnMode.Onboarding).SetActive(true);
			}
		}

		private GameObject GetCamera(PawnMode pawnMode)
		{
			return Object
				.FindObjectsOfType<VirtualCamera>(true)
				.First(x => x.PawnMode == pawnMode)
				.gameObject;
		}
	}
}