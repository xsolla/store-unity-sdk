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
	[SerializeField]
	public Text confirmText;

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

	public MessagePopup ShowError(Error error, Action onClosed = null)
	{
		_onPopupClosed = onClosed;
		errorText.text = error.ToString();
		errorPanel.SetActive(true);
		return this;
	}
	
	public MessagePopup ShowSuccess(Action onClosed = null)
	{
		_onPopupClosed = onClosed;
		successPanel.SetActive(true);
		return this;
	}

	public MessagePopup ShowConfirm(Action onConfirm, Action onCancel)
	{
		_onConfirmPressed = onConfirm;
		_onCancelPressed = onCancel;
		confirmPanel.SetActive(true);
		return this;
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