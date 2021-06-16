using UnityEngine;

namespace Xsolla.Demo
{
	[RequireComponent(typeof(StoreItemUI))]
    public class StoreItemToFullscreen : MonoBehaviour
    {
		[SerializeField] private SimpleButton FullscreenButton = default;

		private void Awake()
		{
			GetComponent<StoreItemUI>().OnInitialized += ShowItemFullscreen;
		}

		private void ShowItemFullscreen(CatalogItemModel itemModel)
		{
			FullscreenButton.onClick += () =>
			{
				StoreItemFullscreenView.ShowItem(itemModel);
			};
		}
	}
}
