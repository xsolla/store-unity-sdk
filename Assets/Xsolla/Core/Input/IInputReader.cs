using System;
using UnityEngine;

namespace Xsolla.Core.Inputs
{
	internal interface IInputReader
	{
		public event Action<char> OnTextInput;

		Vector2 GetCursorPosition();

		Vector2 GetScrollDelta();

		bool IsKeyPressed(KeyCode keyCode);

		bool IsKeyDownThisFrame(KeyCode keyCode);
	}
}