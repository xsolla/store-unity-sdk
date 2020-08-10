using UnityEngine;

[RequireComponent(typeof(LoginPageEnterController))]
public class DemoUserAuthRunner : MonoBehaviour
{
	public static string DemoUser { get; } = "xsolla";

	public void RunDemoUserAuth()
	{
		this.GetComponent<LoginPageEnterController>().RunBasicAuth(username: DemoUser, password: DemoUser, rememberMe: false);
	}
}
