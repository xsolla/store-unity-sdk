#if ENABLE_INPUT_SYSTEM && XSOLLA_UNITY_INPUT_PACKAGE_EXISTS
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Xsolla.Core.Inputs
{
	internal static class InputKeysConverter
	{
		private static readonly Dictionary<KeyCode, Key> KeyCodeToKey = new() {
			{ KeyCode.A, Key.A },
			{ KeyCode.Alpha0, Key.Digit0 },
			{ KeyCode.Alpha1, Key.Digit1 },
			{ KeyCode.Alpha2, Key.Digit2 },
			{ KeyCode.Alpha3, Key.Digit3 },
			{ KeyCode.Alpha4, Key.Digit4 },
			{ KeyCode.Alpha5, Key.Digit5 },
			{ KeyCode.Alpha6, Key.Digit6 },
			{ KeyCode.Alpha7, Key.Digit7 },
			{ KeyCode.Alpha8, Key.Digit8 },
			{ KeyCode.Alpha9, Key.Digit9 },
			{ KeyCode.B, Key.B },
			{ KeyCode.Backspace, Key.Backspace },
			{ KeyCode.C, Key.C },
			{ KeyCode.CapsLock, Key.CapsLock },
			{ KeyCode.D, Key.D },
			{ KeyCode.Delete, Key.Delete },
			{ KeyCode.DownArrow, Key.DownArrow },
			{ KeyCode.E, Key.E },
			{ KeyCode.End, Key.End },
			{ KeyCode.Escape, Key.Escape },
			{ KeyCode.F, Key.F },
			{ KeyCode.F1, Key.F1 },
			{ KeyCode.F10, Key.F10 },
			{ KeyCode.F11, Key.F11 },
			{ KeyCode.F12, Key.F12 },
			{ KeyCode.F2, Key.F2 },
			{ KeyCode.F3, Key.F3 },
			{ KeyCode.F4, Key.F4 },
			{ KeyCode.F5, Key.F5 },
			{ KeyCode.F6, Key.F6 },
			{ KeyCode.F7, Key.F7 },
			{ KeyCode.F8, Key.F8 },
			{ KeyCode.F9, Key.F9 },
			{ KeyCode.G, Key.G },
			{ KeyCode.H, Key.H },
			{ KeyCode.Home, Key.Home },
			{ KeyCode.I, Key.I },
			{ KeyCode.Insert, Key.Insert },
			{ KeyCode.J, Key.J },
			{ KeyCode.K, Key.K },
			{ KeyCode.Keypad0, Key.Numpad0 },
			{ KeyCode.Keypad1, Key.Numpad1 },
			{ KeyCode.Keypad2, Key.Numpad2 },
			{ KeyCode.Keypad3, Key.Numpad3 },
			{ KeyCode.Keypad4, Key.Numpad4 },
			{ KeyCode.Keypad5, Key.Numpad5 },
			{ KeyCode.Keypad6, Key.Numpad6 },
			{ KeyCode.Keypad7, Key.Numpad7 },
			{ KeyCode.Keypad8, Key.Numpad8 },
			{ KeyCode.Keypad9, Key.Numpad9 },
			{ KeyCode.KeypadDivide, Key.NumpadDivide },
			{ KeyCode.KeypadEnter, Key.NumpadEnter },
			{ KeyCode.KeypadMinus, Key.NumpadMinus },
			{ KeyCode.KeypadMultiply, Key.NumpadMultiply },
			{ KeyCode.KeypadPeriod, Key.NumpadPeriod },
			{ KeyCode.KeypadPlus, Key.NumpadPlus },
			{ KeyCode.L, Key.L },
			{ KeyCode.LeftAlt, Key.LeftAlt },
			{ KeyCode.LeftArrow, Key.LeftArrow },
			{ KeyCode.LeftCommand, Key.LeftMeta },
			{ KeyCode.LeftControl, Key.LeftCtrl },
			{ KeyCode.LeftShift, Key.LeftShift },
			{ KeyCode.M, Key.M },
			{ KeyCode.N, Key.N },
			{ KeyCode.O, Key.O },
			{ KeyCode.P, Key.P },
			{ KeyCode.PageDown, Key.PageDown },
			{ KeyCode.PageUp, Key.PageUp },
			{ KeyCode.Q, Key.Q },
			{ KeyCode.R, Key.R },
			{ KeyCode.Return, Key.Enter },
			{ KeyCode.RightAlt, Key.RightAlt },
			{ KeyCode.RightArrow, Key.RightArrow },
			{ KeyCode.RightCommand, Key.RightMeta },
			{ KeyCode.RightControl, Key.RightCtrl },
			{ KeyCode.RightShift, Key.RightShift },
			{ KeyCode.S, Key.S },
			{ KeyCode.Space, Key.Space },
			{ KeyCode.T, Key.T },
			{ KeyCode.Tab, Key.Tab },
			{ KeyCode.U, Key.U },
			{ KeyCode.UpArrow, Key.UpArrow },
			{ KeyCode.V, Key.V },
			{ KeyCode.W, Key.W },
			{ KeyCode.X, Key.X },
			{ KeyCode.Y, Key.Y },
			{ KeyCode.Z, Key.Z }
		};

		public static Key Convert(KeyCode keyCode)
		{
			if (KeyCodeToKey.TryGetValue(keyCode, out var key))
				return key;

			Debug.LogWarning($"KeyCode '{keyCode}' is not mapped to a Key.");
			return Key.None;
		}
	}
}
#endif