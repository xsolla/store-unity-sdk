using UnityEngine;
using UnityEngine.SceneManagement;

public class ExtraController : MonoBehaviour
{
	[SerializeField]
	GameObject signOutButton;
	
	public void Init()
	{
		signOutButton.SetActive(true);

		var btnComponent = signOutButton.GetComponent<SimpleTextButton>();
		btnComponent.onClick = () =>
		{
			SceneManager.LoadScene("Login");
		};
	}
}