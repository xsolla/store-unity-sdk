namespace Xsolla.Core.Popup
{
	public partial class PopupFactory : MonoSingleton<PopupFactory>
	{
		public IBundlePreviewPopup CreateBundlePreview()
		{
			var popup = CreateDefaultPopup(BundlePreviewPopupPrefab, canvas);

			if (popup != null)
				return popup.GetComponent<BundlePreviewPopup>();
			else
				return null;
		}
	}
}
