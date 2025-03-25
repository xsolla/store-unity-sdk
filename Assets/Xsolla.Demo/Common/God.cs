using UnityEngine;
using Xsolla.ReadyToUseStore;

namespace Xsolla.Demo
{
	public class God : MonoBehaviour
	{
		[ContextMenu("Initialize Store")]
		private void OpenStore()
		{
			XsollaReadyToUseStore.Initialize();
		}

		[ContextMenu("Destroy Store")]
		private void DestroyStore()
		{
			XsollaReadyToUseStore.Destroy();
		}
	}
}