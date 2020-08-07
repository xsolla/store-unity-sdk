using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorChanger : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] private Texture2D ButtonHoverCursor;
	[SerializeField] private Texture2D InputFieldHoverCursor;
#pragma warning restore 0649

	private void Awake()
	{
		SimpleButton.OnCursorEnter += ChangeToButtonHoverCursor;
		SimpleButton.OnCursorExit += ChangeBackToDefault;

		InputFieldCursorEventProvider.OnCursorEnter += ChangeToInputFieldHoverCursor;
		InputFieldCursorEventProvider.OnCursorExit += ChangeBackToDefault;
	}

	private void OnDestroy()
	{
		SimpleButton.OnCursorEnter -= ChangeToButtonHoverCursor;
		SimpleButton.OnCursorExit -= ChangeBackToDefault;

		InputFieldCursorEventProvider.OnCursorEnter -= ChangeToInputFieldHoverCursor;
		InputFieldCursorEventProvider.OnCursorExit -= ChangeBackToDefault;
	}

	private void ChangeToButtonHoverCursor() => SetCursorTexture(ButtonHoverCursor);
	private void ChangeToInputFieldHoverCursor() => SetCursorTexture(InputFieldHoverCursor);
	private void ChangeBackToDefault() => SetCursorTexture(null);

	private void SetCursorTexture(Texture2D texture)
	{
		Cursor.SetCursor(texture, Vector2.zero, CursorMode.ForceSoftware);
	}
}
