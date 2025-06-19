#if ENABLE_INPUT_SYSTEM && XSOLLA_UNITY_INPUT_PACKAGE_EXISTS
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Xsolla.Core.Inputs
{
	internal class ModernInputReader : IInputReader
	{
		public event Action<char> OnTextInput;

		public ModernInputReader()
		{
			Keyboard.current.onTextInput += x => OnTextInput?.Invoke(x);
		}

		public Vector2 GetCursorPosition()
		{
			return Mouse.current.position.ReadValue();
		}

		public Vector2 GetScrollDelta()
		{
			return Mouse.current.scroll.ReadValue();
		}

		public bool IsKeyPressed(KeyCode keyCode)
		{
			var key = InputKeysConverter.Convert(keyCode);
			return Keyboard.current[key].isPressed;
		}

		public bool IsKeyDownThisFrame(KeyCode keyCode)
		{
			var key = InputKeysConverter.Convert(keyCode);
			return Keyboard.current[key].wasPressedThisFrame;
		}
	}
}
#endif