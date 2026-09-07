using UnityEngine;

namespace Xsolla.Demo
{
	public class GuyService
	{
		private PawnService PawnService => ServiceLocator.Resolve<PawnService>();

		public GameObject GuyInstance { get; private set; }

		public void SpawnGuy(PawnMode pawnMode)
		{
			SpawnInstance();
			AlignWithMarker();
			ToggleComponents(pawnMode);
		}

		public void DeleteInstance()
		{
			if (GuyInstance)
				PawnService.DestroyPawn(GuyInstance);
		}

		private void SpawnInstance()
		{
			if (GuyInstance)
				DeleteInstance();

			GuyInstance = PawnService.CreatGuy();
		}

		private void AlignWithMarker()
		{
			var marker = Object.FindAnyObjectByType<GuySpawnMarker>().transform;
			var cc = GuyInstance.GetComponent<CharacterController>();
			cc.enabled = false;
			GuyInstance.transform.position = marker.position;
			GuyInstance.transform.rotation = marker.rotation;
			cc.enabled = true;
		}

		private void ToggleComponents(PawnMode mode)
		{
			var components = GuyInstance.GetComponents<IPawnModeComponent>();
			foreach (var component in components)
				component.ToggleMode(mode);
		}
	}
}