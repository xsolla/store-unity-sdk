using System;
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

	Action _onPopupClosed;

	void Awake()
	{
		successButton.onClick = OnPopupButtonClicked;
		errorButton.onClick = OnPopupButtonClicked;
	}

	public void ShowError(Action onClosed = null)
	{
		_onPopupClosed = onClosed; 

		successPanel.SetActive(false);
		errorPanel.SetActive(true);
	}
	
	public void ShowSuccess(Action onClosed = null)
	{
		_onPopupClosed = onClosed;

		successPanel.SetActive(true);
		errorPanel.SetActive(false);
	}

	void OnPopupButtonClicked()
	{
		if (_onPopupClosed != null)
		{
			_onPopupClosed.Invoke();
		}

		Destroy(gameObject);
	}
}