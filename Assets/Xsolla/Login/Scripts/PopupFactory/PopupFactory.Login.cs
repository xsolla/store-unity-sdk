namespace Xsolla.Core.Popup
{
	public partial class PopupFactory : MonoSingleton<PopupFactory>
	{
		public INicknamePopup CreateNickname()
		{
			var popup = CreateDefaultPopup(NicknamePopupPrefab, canvas);

			if (popup != null)
				return popup.GetComponent<NicknamePopup>();
			else
				return null;
		}
	}
}
