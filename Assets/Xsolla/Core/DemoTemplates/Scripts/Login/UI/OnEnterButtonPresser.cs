using UnityEngine;

[RequireComponent(typeof(LogInHotkeys))]
public class OnEnterButtonPresser : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] SimpleButton Button;
#pragma warning restore 0649

	private void Awake()
	{
		this.GetComponent<LogInHotkeys>().EnterKeyPressedEvent += TryPressButton;
	}

	private void TryPressButton()
	{
		if (Button is SimpleButtonLockDecorator lockableButton)
		{
			if (!lockableButton.IsLocked())
				lockableButton.onClick?.Invoke();
		}
		else
		{
			Button.onClick?.Invoke();
		}
	}
}
