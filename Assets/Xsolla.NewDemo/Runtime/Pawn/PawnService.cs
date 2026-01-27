using UnityEngine;

namespace Xsolla.Demo
{
	public class PawnService
	{
		private readonly PawnPrefabRegistry PrefabRegistry;

		public PawnService(PawnPrefabRegistry prefabRegistry)
		{
			PrefabRegistry = prefabRegistry;
		}

		public GameObject CreatGuy()
			=> CreatePawn(PrefabRegistry.Guy);

		public GameObject CreateFox()
			=> CreatePawn(PrefabRegistry.Fox);

		public GameObject CreateOwl()
			=> CreatePawn(PrefabRegistry.Owl);

		public void DestroyPawn(GameObject instance)
		{
			if (instance)
				Object.Destroy(instance);
		}

		private GameObject CreatePawn(GameObject prefab)
		{
			var instance = Object.Instantiate(prefab);
			instance.name = prefab.name;
			return instance;
		}
	}
}