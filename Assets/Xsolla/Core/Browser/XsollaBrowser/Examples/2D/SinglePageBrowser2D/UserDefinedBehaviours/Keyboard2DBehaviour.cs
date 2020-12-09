#if (UNITY_EDITOR || UNITY_STANDALONE)
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Xsolla.Core.Browser
{
	[RequireComponent(typeof(XsollaBrowser))]
	public class KeyboardBehaviour2D : MonoBehaviour
	{
		public event Action EscapePressed;
	
		private static readonly List<KeyCode> BlockedKeyCodes = new List<KeyCode> {
			KeyCode.Escape,
			KeyCode.LeftShift,
			KeyCode.RightShift,
			KeyCode.LeftAlt,
			KeyCode.RightAlt,
			KeyCode.LeftControl,
			KeyCode.RightControl
		};
	
		private static readonly List<KeyCode> AllKeyCodes = Enum.GetValues(typeof(KeyCode)).OfType<KeyCode>()
			.Where(k => !k.ToString().Contains("Mouse")).Except(BlockedKeyCodes)
			.ToList();

		private IXsollaBrowserKeyboardInput keyboardInput;

		private void Awake()
		{
			keyboardInput = gameObject.GetComponent<XsollaBrowser>().Input.Keyboard;
		}
	
		private void Update()
		{
			ManualHandleOfKeyCode(KeyCode.LeftShift, "Shift");
			ManualHandleOfKeyCode(KeyCode.RightShift, "Shift");
			ManualHandleOfKeyCode(KeyCode.LeftAlt, "Alt");
			ManualHandleOfKeyCode(KeyCode.RightAlt, "Alt");
			ManualHandleOfKeyCode(KeyCode.LeftControl, "Control");
			ManualHandleOfKeyCode(KeyCode.RightControl, "Control");

			if (Input.GetKeyUp(KeyCode.Escape)) {
				EscapePressed?.Invoke();
			}

			AllKeyCodes.ForEach(code =>
			{
				if (!Input.GetKeyDown(code)) return;
				var key = KeysConverter.Convert(code);
				keyboardInput.PressKey(key);
			});
		}

		private void ManualHandleOfKeyCode(KeyCode code, string keyName)
		{
			if (Input.GetKeyDown(code))
				keyboardInput.KeyDown(keyName);
			if (Input.GetKeyUp(code))
				keyboardInput.KeyUp(keyName);
		}
	}
}
#endif