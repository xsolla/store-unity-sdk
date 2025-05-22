using System;
using UnityEngine;
using Xsolla.ReadyToUseStore;

namespace Xsolla.Demo
{
	public class God : MonoBehaviour
	{
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Alpha0))
				PlayerPrefs.DeleteAll();

			if (Input.GetKeyDown(KeyCode.Alpha5))
				XsollaReadyToUseStore.OpenStore();

			if (Input.GetKeyDown(KeyCode.Alpha8))
				XsollaReadyToUseStore.CloseStore();
		}
	}
}