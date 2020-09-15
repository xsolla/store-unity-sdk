using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PasswordToggleDecorator : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] Toggle ShowPasswordToggle;
	[SerializeField] Image Eye;
	[SerializeField] Sprite OpenedEye;
	[SerializeField] Sprite ClosedEye;
#pragma warning restore 0649

	private void Start()
	{
		ShowPasswordToggle.onValueChanged.AddListener(ToggleEye);
		ToggleEye(ShowPasswordToggle.isOn);
	}

	private void ToggleEye(bool showPassword)
	{
		Eye.sprite = showPassword ? OpenedEye : ClosedEye;
	}
}
