#if !ENABLE_INPUT_SYSTEM || !XSOLLA_UNITY_INPUT_PACKAGE_EXISTS
using System;
using UnityEngine;

namespace Xsolla.Core.Inputs
{
	internal class ObsoleteInputReader : MonoBehaviour, IInputReader
	{
		public event Action<char> OnTextInput;

		private void Awake()
		{
			DontDestroyOnLoad(this);
		}

		private void OnGUI()
		{
			var e = Event.current;
			if (e.type == EventType.KeyDown && e.character != '\0')
				OnTextInput?.Invoke(e.character);
		}

		public Vector2 GetCursorPosition()
		{
			return Input.mousePosition;
		}

		public Vector2 GetScrollDelta()
		{
			return Input.mouseScrollDelta;
		}

		public bool IsKeyPressed(KeyCode keyCode)
		{
			return Input.GetKey(keyCode);
		}

		public bool IsKeyDownThisFrame(KeyCode keyCode)
		{
			return Input.GetKeyDown(keyCode);
		}
	}
}
#endif