using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	[RequireComponent(typeof(UserProfileEntryUI))]
	public class AndroidProfileEntryDisabler : MonoBehaviour
	{
	#if UNITY_ANDROID
		private void Start()
		{
			DemoController.Instance.LoginDemo.GetUserInfo(Token.Instance,
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
