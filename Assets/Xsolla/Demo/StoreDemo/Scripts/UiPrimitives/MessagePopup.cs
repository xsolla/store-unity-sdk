using UnityEngine;

public class MessagePopup : MonoBehaviour
{
	[SerializeField]
	GameObject successPanel;
	
	[SerializeField]
	GameObject errorPanel;

	[SerializeField]
	SimpleTextButton successButton;
	[SerializeField]
	SimpleTextButton errorButton;

	void Awake()
	{
		successButton.onClick = OnPopupButtonClicked;
		errorButton.onClick = OnPopupButtonClicked;
	}

	public void ShowError()
	{
		successPanel.SetActive(false);
		errorPanel.SetActive(true);
	}
	
	public void ShowSuccess()
	{
		successPanel.SetActive(true);
		errorPanel.SetActive(false);
	}

	void OnPopupButtonClicked()
	{
		Destroy(gameObject);
	}
}