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
	GameObject confirmPanel;

	[SerializeField]
	SimpleTextButton successButton;
	[SerializeField]
	SimpleTextButton errorButton;
	[SerializeField]
	SimpleTextButton confirmButton;
	[SerializeField]
	SimpleTextButton cancelButton;

	[SerializeField]
	Text errorText;
	
	Action _onPopupClosed;
	Action _onConfirmPressed;
	Action _onCancelPressed;

	void Awake()
	{
		DisablePanels();
		successButton.onClick = OnPopupButtonClicked;
		errorButton.onClick = OnPopupButtonClicked;
		confirmButton.onClick = OnConfirmButtonClicked;
		cancelButton.onClick = OnCancelButtonClicked;
	}

	void DisablePanels()
	{
		successPanel?.SetActive(false);
		errorPanel?.SetActive(false);
		confirmPanel?.SetActive(false);
	}

	public void ShowError(Error error, Action onClosed = null)
	{
		_onPopupClosed = onClosed;
		errorText.text = error.ToString();
		errorPanel.SetActive(true);
	}
	
	public void ShowSuccess(Action onClosed = null)
	{
		_onPopupClosed = onClosed;
		successPanel.SetActive(true);
	}

	public void ShowConfirm(Action onConfirm, Action onCancel)
	{
		_onConfirmPressed = onConfirm;
		_onCancelPressed = onCancel;
		confirmPanel.SetActive(true);
	}

	void OnConfirmButtonClicked()
	{
		AnyButtonPressed(_onConfirmPressed);
	}

	void OnCancelButtonClicked()
	{
		AnyButtonPressed(_onCancelPressed);
	}

	void OnPopupButtonClicked()
	{
		AnyButtonPressed(_onPopupClosed);
	}

	void AnyButtonPressed(Action callback)
	{
		callback?.Invoke();
		Destroy(gameObject);
	}
}