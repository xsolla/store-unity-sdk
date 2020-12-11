using UnityEngine;

namespace Xsolla.Demo
{
	[RequireComponent(typeof(UserProfileEntryUI))]
	public class AndroidProfileEntryDisabler : MonoBehaviour
	{
	#if UNITY_ANDROID
		private void Start()
		{
			var token = DemoController.Instance.LoginDemo.Token;

			DemoController.Instance.LoginDemo.GetUserInfo(token,
				onSuccess: info =>
				{
					if (string.IsNullOrEmpty(info.email))
					{
						this.gameObject.SetActive(false);
					}
				},
				onError: error => Debug.Log("Could not get user info"));
		}
	#endif
	}
}
