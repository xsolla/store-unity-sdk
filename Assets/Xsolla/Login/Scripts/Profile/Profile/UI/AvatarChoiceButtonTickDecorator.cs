using System;
using UnityEngine;

namespace Xsolla.Demo
{
	[RequireComponent(typeof(SimpleButton))]
	public class AvatarChoiceButtonTickDecorator : MonoBehaviour
	{
		[SerializeField] GameObject Tick;

		public static event Action<AvatarChoiceButtonTickDecorator> AvatarChoiceButtonPressed;

		private void Awake()
		{
			GetComponent<SimpleButton>().onClick += () =>
			{
				if (AvatarChoiceButtonPressed != null)
					AvatarChoiceButtonPressed.Invoke(this);
			};
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
}
