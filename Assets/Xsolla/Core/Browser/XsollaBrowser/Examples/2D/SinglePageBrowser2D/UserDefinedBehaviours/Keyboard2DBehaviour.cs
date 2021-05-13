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

		private static List<KeyCode> SystemKeys = new List<KeyCode>
		{
			KeyCode.Escape
		};
	
		private static readonly List<KeyValuePair<KeyCode, string>> ModificationKeys = new List<KeyValuePair<KeyCode, string>>
		{
			new KeyValuePair<KeyCode, string>(KeyCode.LeftShift,	"Shift"),
			new KeyValuePair<KeyCode, string>(KeyCode.RightShift,	"Shift"),
			new KeyValuePair<KeyCode, string>(KeyCode.LeftAlt,		"Alt"),
			new KeyValuePair<KeyCode, string>(KeyCode.RightAlt,		"Alt"),
			new KeyValuePair<KeyCode, string>(KeyCode.LeftControl,  "Control"),
			new KeyValuePair<KeyCode, string>(KeyCode.RightControl, "Control")
		};

		private static readonly List<KeyValuePair<KeyCode, string>> NumpadKeys = new List<KeyValuePair<KeyCode, string>>
		{
			new KeyValuePair<KeyCode, string>(KeyCode.Keypad0, "Digit0"),
			new KeyValuePair<KeyCode, string>(KeyCode.Keypad1, "Digit1"),
			new KeyValuePair<KeyCode, string>(KeyCode.Keypad2, "Digit2"),
			new KeyValuePair<KeyCode, string>(KeyCode.Keypad3, "Digit3"),
			new KeyValuePair<KeyCode, string>(KeyCode.Keypad4, "Digit4"),
			new KeyValuePair<KeyCode, string>(KeyCode.Keypad5, "Digit5"),
			new KeyValuePair<KeyCode, string>(KeyCode.Keypad6, "Digit6"),
			new KeyValuePair<KeyCode, string>(KeyCode.Keypad7, "Digit7"),
			new KeyValuePair<KeyCode, string>(KeyCode.Keypad8, "Digit8"),
			new KeyValuePair<KeyCode, string>(KeyCode.Keypad9, "Digit9")
		};

		private static readonly List<KeyCode> AllKeyCodes = Enum.GetValues(typeof(KeyCode)).OfType<KeyCode>()
			.Where(key => (key < KeyCode.Mouse0))
			.Except(SystemKeys).Except(GetKeys(ModificationKeys)).Except(GetKeys(NumpadKeys))
			.ToList();

		private IXsollaBrowserKeyboardInput keyboardInput;

		private void Awake()
		{
			keyboardInput = gameObject.GetComponent<XsollaBrowser>().Input.Keyboard;
		}
	
		private void Update()
		{
			if (InputProxy.GetKeyUp(KeyCode.Escape))
			{
				EscapePressed?.Invoke();
			}

			foreach (var pair in ModificationKeys)
			{
				if (InputProxy.GetKeyDown(pair.Key))
					keyboardInput.KeyDown(pair.Value);

				if (InputProxy.GetKeyUp(pair.Key))
					keyboardInput.KeyUp(pair.Value);
			}

			foreach (var pair in NumpadKeys)
			{
				if (InputProxy.GetKeyDown(pair.Key))
					keyboardInput.PressKey(pair.Value);
			}

			AllKeyCodes.ForEach(code =>
			{
				if (InputProxy.GetKeyDown(code))
				{
					var key = KeysConverter.Convert(code);
					keyboardInput.PressKey(key);
				}
			});
		}

		private static IEnumerable<KeyCode> GetKeys(List<KeyValuePair<KeyCode, string>> keyValueCollection)
		{
			List<KeyCode> keys = new List<KeyCode>(keyValueCollection.Count);

			foreach (var keyValuePair in keyValueCollection)
				keys.Add(keyValuePair.Key);

			return keys;
		}
	}
}
#endif
