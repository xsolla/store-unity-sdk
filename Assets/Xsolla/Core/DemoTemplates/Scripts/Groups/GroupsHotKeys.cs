using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupsHotKeys : MonoBehaviour
{
	public Action ArrowUpKeyPressedEvent;
	public Action ArrowDownKeyPressedEvent;

	void Start()
	{
		gameObject.AddComponent<HotkeyCoroutine>()
			.StartCoroutine(KeyCode.DownArrow, 0.1F)
			.KeyPressedEvent += () =>
			{
				if (ArrowDownKeyPressedEvent != null)
					ArrowDownKeyPressedEvent.Invoke();
			};
		gameObject.AddComponent<HotkeyCoroutine>()
			.StartCoroutine(KeyCode.UpArrow, 0.1F)
			.KeyPressedEvent += () =>
			{
				if (ArrowUpKeyPressedEvent != null)
					ArrowUpKeyPressedEvent.Invoke();
			};
	}
}