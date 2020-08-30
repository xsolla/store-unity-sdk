using UnityEngine;

public class OpenUrlOnClick : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] private SimpleButton Button;
	[SerializeField] private UrlType UrlType;
#pragma warning restore 0649

	private string _url;

	public string URL
	{
		get => _url;
		set => _url = value;
	}

	private void Awake()
	{
		Button.onClick += OpenUrl;
		URL = DemoController.Instance.UrlContainer.GetUrl(UrlType);
	}

	private void OpenUrl()
	{
		Application.OpenURL(URL);
	}
}
