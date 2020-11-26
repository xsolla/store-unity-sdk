using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

public class CouponRewardItemUI : MonoBehaviour
{
	[SerializeField] Image itemImage;
	[SerializeField] Text itemName;
	[SerializeField] Text itemDescription;
	[SerializeField] Text itemQuantity;

	public void Initialize(CouponRedeemedItemModel rewardItem)
	{
		itemName.text = rewardItem.Name;
		itemDescription.text = rewardItem.Description;
		itemQuantity.text = rewardItem.Quantity.ToString();

		if (itemImage.sprite != null)
		{
			if (!string.IsNullOrEmpty(rewardItem.ImageUrl))
			{
				ImageLoader.Instance.GetImageAsync(rewardItem.ImageUrl, LoadImageCallback);
			}
			else
			{
				Debug.LogError($"Coupon reward item item with sku = '{rewardItem.Sku}' without image!");
			}
		}
	}

	void LoadImageCallback(string url, Sprite image)
	{
		itemImage.sprite = image;
	}
}