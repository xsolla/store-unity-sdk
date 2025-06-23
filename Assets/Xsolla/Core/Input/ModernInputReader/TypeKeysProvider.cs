#if ENABLE_INPUT_SYSTEM && XSOLLA_UNITY_INPUT_PACKAGE_EXISTS
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Xsolla.Core.Inputs
{
	internal static class TypeKeysProvider
	{
		private static HashSet<KeyControl> _keys;

		public static HashSet<KeyControl> TypedKeys => _keys ??= GetTypedKeys();

		private static HashSet<KeyControl> GetTypedKeys()
		{
			return Keyboard
				.current
				.allKeys
				.Where(x => !ExcludedKeyCodes.Contains(x.keyCode))
				.ToHashSet();
		}

		private static readonly HashSet<Key> ExcludedKeyCodes = new() {
			// Modifier
			Key.LeftAlt,
			Key.RightAlt,
			Key.LeftCtrl,
			Key.RightCtrl,
			Key.LeftShift,
			Key.RightShift,
			Key.LeftMeta,
			Key.RightMeta,
			Key.CapsLock,

			// Control
			Key.Enter,
			Key.Escape,
			Key.Tab,
			Key.Backspace,
			Key.Space,

			// Navigation
			Key.UpArrow,
			Key.DownArrow,
			Key.LeftArrow,
			Key.RightArrow,
			Key.Insert,
			Key.Delete,
			Key.Home,
			Key.End,
			Key.PageUp,
			Key.PageDown,

			// Function
			Key.F1,
			Key.F2,
			Key.F3,
			Key.F4,
			Key.F5,
			Key.F6,
			Key.F7,
			Key.F8,
			Key.F9,
			Key.F10,
			Key.F11,
			Key.F12,

			// Numpad operations
			Key.NumpadEnter,
			Key.NumpadPlus,
			Key.NumpadMinus,
			Key.NumpadMultiply,
			Key.NumpadDivide,
			Key.NumpadPeriod
		};
	}
}
#endif