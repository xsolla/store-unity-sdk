using System;
using UnityEngine;

public class ProfileAccountLinkButtons : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] SimpleButton _getAccountLinkButton;
	[SerializeField] SimpleButton _accountLinkingButton;
#pragma warning restore 0649
	
	public SimpleButton GetAccountLinkButton => _getAccountLinkButton;
	public SimpleButton AccountLinkingButton => _accountLinkingButton;

	public Action OnGetAccountLinkButtonClick { get; set; }
	public Action OnAccountLinkingButtonClick { get; set; }

	private void Awake()
	{
		_getAccountLinkButton.onClick += () => OnGetAccountLinkButtonClick?.Invoke();
		_accountLinkingButton.onClick += () => OnAccountLinkingButtonClick?.Invoke();
	}

	public void ActivateButton(SimpleButton button)
	{
		if (object.ReferenceEquals(button, _getAccountLinkButton))
		{
			SetButtonsActive(isGetAccountLinkActive: true, isAccountLinkingActive: false);
		}

		if (object.ReferenceEquals(button, _accountLinkingButton))
		{
			SetButtonsActive(isGetAccountLinkActive: false, isAccountLinkingActive: true);
		}
	}

	private void SetButtonsActive(bool isGetAccountLinkActive, bool isAccountLinkingActive)
	{
		_getAccountLinkButton.gameObject.SetActive(isGetAccountLinkActive);
		_accountLinkingButton.gameObject.SetActive(isAccountLinkingActive);
	}
}
