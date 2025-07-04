using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.XsollaBrowser
{
	public class KeyboardHandler : MonoBehaviour, IBrowserHandler
	{
		private BrowserPage Page;
		private Dictionary<KeyCode, string> NavigationKeys;

		private void OnApplicationFocus(bool hasFocus)
		{
			if (hasFocus && Page != null)
				Page.UpKeyAsync("Alt");
		}

		public void Run(BrowserPage page, CancellationToken cancellationToken)
		{
			Page = page;

			NavigationKeys = new Dictionary<KeyCode, string> {
				[KeyCode.Escape] = "Escape",
				[KeyCode.LeftArrow] = "ArrowLeft",
				[KeyCode.RightArrow] = "ArrowRight",
				[KeyCode.UpArrow] = "ArrowUp",
				[KeyCode.DownArrow] = "ArrowDown",
				[KeyCode.Home] = "Home",
				[KeyCode.End] = "End",
				[KeyCode.PageUp] = "PageUp",
				[KeyCode.PageDown] = "PageDown",
				[KeyCode.Backspace] = "Backspace",
				[KeyCode.Delete] = "Delete",
				[KeyCode.Tab] = "Tab",
				[KeyCode.KeypadEnter] = "Enter",
			};

			StartCoroutine(HandleInput(cancellationToken));

			InputProvider.OnTextInput += OnTextInput;
		}

		public void Stop()
		{
			InputProvider.OnTextInput -= OnTextInput;
			StopAllCoroutines();
		}

		private void OnTextInput(char character)
		{
			if (GetNavigationKeyCodeDownThisFrame().HasValue)
				return;
			
			if (GetClipboardKeyCodeDownThisFrame())
				return;

			if (!char.IsControl(character))
				Page?.SendCharacterAsync(character.ToString());
		}

		private IEnumerator HandleInput(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				if (!InputProvider.IsKeyDownThisFrame(KeyCode.Escape))
					ProcessInput();

				yield return null;
			}
		}

		private void ProcessInput()
		{
			if (ProcessNavigation())
				return;

			ProcessClipboard();
		}

		private KeyCode? GetNavigationKeyCodeDownThisFrame()
		{
			foreach (var keyCode in NavigationKeys.Keys)
			{
				if (InputProvider.IsKeyDownThisFrame(keyCode))
					return keyCode;
			}

			return null;
		}

		private bool GetClipboardKeyCodeDownThisFrame()
		{
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
			if ((InputProvider.IsKeyPressed(KeyCode.LeftCommand) || InputProvider.IsKeyPressed(KeyCode.RightCommand)) && InputProvider.IsKeyDownThisFrame(KeyCode.V))
			{
				return true;
			}
#else
			if ((InputProvider.IsKeyPressed(KeyCode.LeftControl) || InputProvider.IsKeyPressed(KeyCode.RightControl)) && InputProvider.IsKeyDownThisFrame(KeyCode.V))
			{
				return true;
			}
#endif
			return false;
		}

		private bool ProcessNavigation()
		{
			var keyCode = GetNavigationKeyCodeDownThisFrame();
			if (keyCode.HasValue)
			{
				var key = NavigationKeys[keyCode.Value];
				Page?.DownKeyAsync(key);
				return true;
			}

			return false;
		}

		private void ProcessClipboard()
		{
			if (GetClipboardKeyCodeDownThisFrame())
			{
				PasteFromClipboard();
			}
		}

		private void PasteFromClipboard()
		{
			var text = GUIUtility.systemCopyBuffer;
			if (!string.IsNullOrEmpty(text))
				Page.SendCharacterAsync(text);
		}
	}
}