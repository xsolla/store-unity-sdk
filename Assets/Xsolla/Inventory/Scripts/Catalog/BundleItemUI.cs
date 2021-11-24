using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class BundleItemUI : MonoBehaviour
	{
		[SerializeField] private Image itemImage;
		[SerializeField] private Text itemName;
		[SerializeField] private Text itemDescription;
		[SerializeField] private Text itemQuantity;

		public void Initialize(BundleContentItem item)
		{
			itemName.text = item.Name;
			itemDescription.text = item.Description;
			itemQuantity.text = item.Quantity.ToString();

			if (!string.IsNullOrEmpty(item.ImageUrl))
			{
				ImageLoader.Instance.GetImageAsync(item.ImageUrl, LoadImageCallback);
			}
			else
			{
				Debug.LogError(string.Format("Bundle content item with sku = '{0}' without image!", item.Sku));
			}
		}

		void LoadImageCallback(string url, Sprite image)
		{
			if (itemImage)
				itemImage.sprite = image;
		}
	}
}
