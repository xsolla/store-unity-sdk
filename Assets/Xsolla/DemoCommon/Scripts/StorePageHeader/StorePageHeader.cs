using UnityEngine;

namespace Xsolla.Demo
{
	public class StorePageHeader : MonoBehaviour
	{
		[SerializeField] private GameObject userInfo = default;

		void Awake()
		{
			if (DemoController.Instance.IsAccessTokenAuth)
			{
				userInfo.SetActive(false);
			}
		}
	}
}
