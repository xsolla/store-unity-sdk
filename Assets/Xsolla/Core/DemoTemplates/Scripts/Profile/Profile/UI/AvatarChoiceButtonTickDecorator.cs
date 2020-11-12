using System;
using UnityEngine;

[RequireComponent(typeof(SimpleButton))]
public class AvatarChoiceButtonTickDecorator : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] GameObject Tick;
#pragma warning restore 0649

	public static event Action<AvatarChoiceButtonTickDecorator> AvatarChoiceButtonPressed;

	private void Awake()
	{
		GetComponent<SimpleButton>().onClick += () => AvatarChoiceButtonPressed?.Invoke(this);
		AvatarChoiceButtonPressed += OnAvatarChoiceButtonPressed;
	}

	private void OnDestroy()
	{
		AvatarChoiceButtonPressed -= OnAvatarChoiceButtonPressed;
	}

	private void OnAvatarChoiceButtonPressed(AvatarChoiceButtonTickDecorator triggeredInstance)
	{
		if (triggeredInstance == this)
			Tick.SetActive(true);
		else
			Tick.SetActive(false);
	}
}
