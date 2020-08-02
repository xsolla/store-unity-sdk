using UnityEngine;

public class OpenUrlOnClick : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] private SimpleButton[] Buttons;
	[SerializeField] private string URL;
#pragma warning restore 0649

	private void Awake()
	{
		foreach (var button in Buttons)
			button.onClick += () => Application.OpenURL(URL);
	}
}
