using System;
using UnityEngine;

public class StoreTabsHotkey : MonoBehaviour
{
	public Action TabKeyPressedEvent;
	public Action LeftArrowKeyPressedEvent;
	public Action RightArrowKeyPressedEvent;

	void Start()
	{
		gameObject.AddComponent<HotkeyCoroutine>()
			.StartCoroutine(KeyCode.Tab)
			.KeyPressedEvent += () =>
			{
				if (TabKeyPressedEvent != null)
					TabKeyPressedEvent.Invoke();
			};
		gameObject.AddComponent<HotkeyCoroutine>()
			.StartCoroutine(KeyCode.LeftArrow)
			.KeyPressedEvent += () =>
			{
				if (LeftArrowKeyPressedEvent != null)
					LeftArrowKeyPressedEvent.Invoke();
			};
		gameObject.AddComponent<HotkeyCoroutine>()
			.StartCoroutine(KeyCode.RightArrow)
			.KeyPressedEvent += () =>
			{
				if (RightArrowKeyPressedEvent != null)
					RightArrowKeyPressedEvent.Invoke();
			};
	}
}
