using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SimpleButton))]
public class AvatarChoiceButton : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] Image AvatarImage;
#pragma warning restore 0649

	public static event Action<Sprite> AvatarPicked;

	private void Awake()
	{
		GetComponent<SimpleButton>().onClick += RaiseAvatarPicked;
	}

	private void RaiseAvatarPicked()
	{
		AvatarPicked?.Invoke(AvatarImage.sprite);
	}
}
