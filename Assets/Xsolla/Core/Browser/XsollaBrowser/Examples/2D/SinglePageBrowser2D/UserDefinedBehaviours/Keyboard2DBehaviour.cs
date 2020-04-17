#if (UNITY_EDITOR || UNITY_STANDALONE)
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

[RequireComponent(typeof(XsollaBrowser))]
public class KeyboardBehaviour2D : MonoBehaviour
{
	public event Action EscapePressed;
	
	private static readonly List<KeyCode> BlockedKeyCodes = new List<KeyCode> {
		KeyCode.Escape,
		KeyCode.LeftShift,
		KeyCode.RightShift
	};
	
	private static readonly List<KeyCode> AllKeyCodes = Enum.GetValues(typeof(KeyCode)).OfType<KeyCode>()
		.Where(k => !k.ToString().Contains("Mouse"))
		.Except(BlockedKeyCodes)
		.ToList();

	private IXsollaBrowserKeyboardInput keyboardInput;

	private void Awake()
	{
		keyboardInput = gameObject.GetComponent<XsollaBrowser>().Input.Keyboard;
	}

	private void Update()
    {
	    if (Input.GetKeyUp(KeyCode.Escape)) {
		    EscapePressed?.Invoke();
	    }

	    AllKeyCodes.ForEach(code =>
	    {
		    if (!Input.GetKeyDown(code)) return;
		    Debug.Log("KeyCode up: " + code);
		    var key = ConvertKeyCode(code);
		    keyboardInput.PressKey(key);
	    });
    }

	private static string ConvertKeyCode(KeyCode code)
	{
		return IsLetterKey(code) ? ConvertLetter(code) : KeysConverter.Convert(code);
	}

	private static string ConvertLetter(KeyCode code)
	{
		return IsShiftPressed() ? code.ToString() : code.ToString().ToLower();
	}

	private static bool IsShiftPressed()
	{
		return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
	}
	
	private static bool IsLetterKey(KeyCode code)
	{
		return code >= KeyCode.A && code <= KeyCode.Z;
	}
}
#endif