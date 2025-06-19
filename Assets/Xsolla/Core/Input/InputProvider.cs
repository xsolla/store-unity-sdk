using System;
using UnityEngine;
using Xsolla.Core.Inputs;

namespace Xsolla.Core
{
	public static class InputProvider
	{
		public static event Action<char> OnTextInput
		{
			add => Reader.OnTextInput += value;
			remove => Reader.OnTextInput -= value;
		}

		public static Vector2 GetCursorPosition()
			=> Reader.GetCursorPosition();

		public static Vector2 GetScrollDelta()
			=> Reader.GetScrollDelta();

		public static bool IsKeyPressed(KeyCode keyCode)
			=> Reader.IsKeyPressed(keyCode);

		public static bool IsKeyDownThisFrame(KeyCode keyCode)
			=> Reader.IsKeyDownThisFrame(keyCode);

		#region Singleton

		private static IInputReader _reader;

		private static IInputReader Reader => _reader ??= CreateReader();

		private static IInputReader CreateReader()
		{
#if ENABLE_INPUT_SYSTEM && XSOLLA_UNITY_INPUT_PACKAGE_EXISTS
			return new ModernInputReader();
#else
			return new GameObject("ObsoleteInputReaderHandler").AddComponent<ObsoleteInputReader>();
#endif
		}

		#endregion
	}
}