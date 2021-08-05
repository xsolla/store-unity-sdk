using UnityEngine;

namespace Xsolla.Demo
{
	public class PasswordProfileEntryDisabler : MonoBehaviour
	{
		private void Start()
		{
			var token = Core.Token.Instance;

			DemoController.Instance.LoginDemo.GetUserInfo(token,
				onSuccess: info =>
				{
					var isEmailPresented = !string.IsNullOrEmpty(info.email);
					var isUsernamePresented = !string.IsNullOrEmpty(info.username);

					if (!isEmailPresented && !isUsernamePresented)
					{
						this.gameObject.SetActive(false);
					}
				},
				onError: error => Debug.Log("Could not get user info"));
		}
	}
}	
