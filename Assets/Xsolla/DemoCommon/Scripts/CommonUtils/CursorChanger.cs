using UnityEngine;

namespace Xsolla.Demo
{
	public class CursorChanger : MonoBehaviour
	{
		[SerializeField] private Texture2D ButtonHoverCursor = default;
		[SerializeField] private Texture2D InputFieldHoverCursor = default;

		private void Awake()
		{
			SimpleButton.OnCursorEnter += ChangeToButtonHoverCursor;
			SimpleButton.OnCursorExit += ChangeBackToDefault;
		
			AddToCartButton.OnCursorEnter += ChangeToButtonHoverCursor;
			AddToCartButton.OnCursorExit += ChangeBackToDefault;

			MenuButton.OnCursorEnter += ChangeToButtonHoverCursor;
			MenuButton.OnCursorExit += ChangeBackToDefault;

			InputFieldCursorEventProvider.OnCursorEnter += ChangeToInputFieldHoverCursor;
			InputFieldCursorEventProvider.OnCursorExit += ChangeBackToDefault;
		
			BlurSelectedUser.OnActivated += ChangeToButtonHoverCursor;
			BlurSelectedUser.OnDeactivated += ChangeBackToDefault;
		}

		private void OnDestroy()
		{
			SimpleButton.OnCursorEnter -= ChangeToButtonHoverCursor;
			SimpleButton.OnCursorExit -= ChangeBackToDefault;
		
			AddToCartButton.OnCursorEnter -= ChangeToButtonHoverCursor;
			AddToCartButton.OnCursorExit -= ChangeBackToDefault;

			MenuButton.OnCursorEnter -= ChangeToButtonHoverCursor;
			MenuButton.OnCursorExit -= ChangeBackToDefault;

			InputFieldCursorEventProvider.OnCursorEnter -= ChangeToInputFieldHoverCursor;
			InputFieldCursorEventProvider.OnCursorExit -= ChangeBackToDefault;
		
			BlurSelectedUser.OnActivated -= ChangeToButtonHoverCursor;
			BlurSelectedUser.OnDeactivated -= ChangeBackToDefault;
		}

		private void ChangeToButtonHoverCursor() => SetCursorTexture(ButtonHoverCursor);
		
		private void ChangeToInputFieldHoverCursor() => SetCursorTexture(InputFieldHoverCursor);
		
		public static void ChangeBackToDefault() => SetCursorTexture(null);
		
		public static void SetCursorTexture(Texture2D texture)
		{
#if !ENABLE_INPUT_SYSTEM
			Cursor.SetCursor(texture, Vector2.zero, CursorMode.ForceSoftware);
#endif
		}
	}
}
