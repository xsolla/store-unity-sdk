using UnityEngine;
using UnityEngine.UI;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.ReadyToUseStore
{
	public class CatalogItemWidget : MonoBehaviour, ICatalogItemWidget
	{
		[SerializeField] private Text Text;
		[SerializeField] private Image IconImage;
		[SerializeField] private Button Button;

		public void Construct(StoreItem item)
		{
			Text.text = item.name;
			ImageLoader.LoadSprite(item.image_url, sprite => IconImage.sprite = sprite);

			Button.onClick.AddListener(() => Purchase(item.sku));
		}

		private void Purchase(string sku)
		{
			XsollaCatalog
				.Purchase(sku,
					x => Debug.Log("Purchase success: " + x),
					e => Debug.LogError("Purchase error: " + e));
		}
	}
}