using UnityEngine;

namespace Xsolla.Demo
{
	public class OnAccessTokenAuthDisabler : MonoBehaviour
	{
		void Awake()
		{
			if (DemoController.Instance.IsAccessTokenAuth)
				this.gameObject.SetActive(false);
		}
	}
}
