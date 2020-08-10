using UnityEngine;

public class LoginPageControllerDebug : MonoBehaviour
{
	private void Awake()
	{
		var loginPage = GetComponent<LoginPageController>();

		loginPage.OnSuccess += () => Debug.Log($"LoginPageControllerDebug: SUCCESSFUL ACTION");
		loginPage.OnError += error => Debug.LogError($"LoginPageControllerDebug: ACTION ERROR: {error}");
	}
}