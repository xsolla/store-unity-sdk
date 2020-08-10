using UnityEngine;

public class OpenUrlOnClick : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] private SimpleButton[] Buttons;
	[SerializeField] private string _url;
#pragma warning restore 0649

	public string URL
	{
		get => _url;
		set => _url = value;
	}

	private void Awake()
	{
		foreach (var button in Buttons)
			button.onClick += () => Application.OpenURL(_url);
	}
}
