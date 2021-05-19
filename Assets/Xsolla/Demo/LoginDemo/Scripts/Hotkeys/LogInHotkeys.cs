using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LogInHotkeys : MonoBehaviour
{
    public event Action EnterKeyPressedEvent;
    public event Action TabKeyPressedEvent;

	private void Start()
	{
		gameObject.AddComponent<HotkeyCoroutine>()
			.StartCoroutine(KeyCode.Return, 0.2F)
			.KeyPressedEvent += () =>
			{
				if (EnterKeyPressedEvent != null)
					EnterKeyPressedEvent.Invoke();
			};

		gameObject.AddComponent<HotkeyCoroutine>()
			.StartCoroutine(KeyCode.Tab, 0.2F)
			.KeyPressedEvent += () =>
			{
				if (TabKeyPressedEvent != null)
					TabKeyPressedEvent.Invoke();
			};
	}

	private void OnDestroy()
	{
		List<HotkeyCoroutine> hotkeys = gameObject.GetComponents<HotkeyCoroutine>().ToList();
		hotkeys.ForEach(h => Destroy(h));
	}
}
