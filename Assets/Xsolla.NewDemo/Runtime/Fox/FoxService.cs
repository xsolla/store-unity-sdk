using System.Linq;
using UnityEngine;

namespace Xsolla.Demo
{
	public class FoxService
	{
		private static PawnService PawnService => ServiceLocator.Resolve<PawnService>();

		private GameObject FoxInstance { get; set; }

		public void SpawnFox(PawnMode mode)
		{
			SpawnInstance();
			AlignWithMarker(mode);
			ToggleComponents(mode);
		}

		public void DeleteInstance()
		{
			PawnService.DestroyPawn(FoxInstance);
		}

		private void SpawnInstance()
		{
			if (FoxInstance)
				DeleteInstance();

			FoxInstance = PawnService.CreateFox();
		}

		private void AlignWithMarker(PawnMode pawnMode)
		{
			var marker = Object
				.FindObjectsByType<FoxSpawnMarker>(FindObjectsSortMode.None)
				.First(x => x.PawnMode == pawnMode)
				.transform;

			FoxInstance.transform.position = marker.position;
			FoxInstance.transform.rotation = marker.rotation;
		}

		private void ToggleComponents(PawnMode pawnMode)
		{
			var components = FoxInstance.GetComponents<IPawnModeComponent>();
			foreach (var component in components)
				component.ToggleMode(pawnMode);
		}
	}
}