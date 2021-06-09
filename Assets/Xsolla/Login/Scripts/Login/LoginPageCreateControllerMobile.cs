using UnityEngine;
using Xsolla.Demo;

public class LoginPageCreateControllerMobile : MonoBehaviour
{
	[SerializeField] SimpleButton loginButton = default;

	public static bool IsLoginNavigationTriggered { get; private set; }

	void Awake()
	{
		IsLoginNavigationTriggered = false;

		loginButton.onClick += () =>
		{
			IsLoginNavigationTriggered = true;
			DemoController.Instance.SetPreviousState();
		};
	}
}