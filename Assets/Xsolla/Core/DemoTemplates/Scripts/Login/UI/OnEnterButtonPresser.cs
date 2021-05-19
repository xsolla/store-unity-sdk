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
		if (Button is SimpleButtonLockDecorator)
		{
			var lockableButton = (SimpleButtonLockDecorator)Button;
			if (!lockableButton.IsLocked())
			{
				if (lockableButton.onClick != null)
					lockableButton.onClick.Invoke();
			}
		}
		else
		{
			if (Button.onClick != null)
				Button.onClick.Invoke();
		}
	}
}
