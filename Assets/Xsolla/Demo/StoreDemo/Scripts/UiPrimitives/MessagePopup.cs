using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

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

	[SerializeField]
	Text errorText;
	
	Action _onPopupClosed;

	void Awake()
	{
		successButton.onClick = OnPopupButtonClicked;
		errorButton.onClick = OnPopupButtonClicked;
	}

	public void ShowError(Error error, Action onClosed = null)
	{
		_onPopupClosed = onClosed;

		errorText.text = error.ToString();

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