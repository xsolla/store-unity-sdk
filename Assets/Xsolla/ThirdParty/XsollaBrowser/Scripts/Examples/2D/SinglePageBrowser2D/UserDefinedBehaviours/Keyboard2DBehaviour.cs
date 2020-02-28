using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

[RequireComponent(typeof(XsollaBrowser))]
public class KeyboardBehaviour2D : MonoBehaviour
{
	public event Action EscapePressed;

	private List<KeyCode> allKeyCodes = Enum.GetValues(typeof(KeyCode)).OfType<KeyCode>()
		.Where(k => !k.ToString().Contains("Mouse"))
		.ToList();

	private List<KeyCode> blockedKeyCodes = new List<KeyCode> {
		KeyCode.Escape
	};

	private IXsollaBrowserKeyboardInput keyboardInput;

	private void Awake()
	{
		keyboardInput = gameObject.GetComponent<XsollaBrowser>().Input.Keyboard;
		allKeyCodes = allKeyCodes.Except(blockedKeyCodes).ToList();
	}

	private void Update()
    {
        foreach (KeyCode kcode in allKeyCodes) {
            if (Input.GetKeyUp(kcode)) {
                Debug.Log("KeyCode up: " + kcode);
                string key = KeysConverter.Convert(kcode);
                keyboardInput.PressKey(key);
            }
        }
		if (Input.GetKeyUp(KeyCode.Escape)) {
			EscapePressed?.Invoke();
		}
	}
}
