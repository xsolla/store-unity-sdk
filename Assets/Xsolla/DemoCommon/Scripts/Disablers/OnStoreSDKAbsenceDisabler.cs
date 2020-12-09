using UnityEngine;

namespace Xsolla.Demo
{
	public class OnStoreSDKAbsenceDisabler : MonoBehaviour
	{
		private void Awake()
		{
			if (DemoController.Instance.StoreDemo == null)
				this.gameObject.SetActive(false);
		}
	}
}
