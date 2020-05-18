using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Store;

public class SubscriptionItemUI : MonoBehaviour
{
	private const string SUBSCRIPTION_STATUS_NONE = "none";
	private const string SUBSCRIPTION_STATUS_ACTIVE = "active";
	private const string SUBSCRIPTION_STATUS_EXPIRED = "expired";
	
	[SerializeField]
	Image itemImage;
	[SerializeField]
	GameObject loadingCircle;
	[SerializeField]
	Text itemName;
	[SerializeField]
	Text itemDescription;
	[SerializeField]
	Text itemStatus;
	[SerializeField]
	Text itemExpiration;

	SubscriptionItem _itemInformation;

	public void Initialize(SubscriptionItem itemInformation)
	{
		_itemInformation = itemInformation;

		itemName.text = _itemInformation.name;
		itemDescription.text = _itemInformation.description;

		itemStatus.text = _itemInformation.status.ToUpper();

		switch (_itemInformation.status)
		{
			case SUBSCRIPTION_STATUS_NONE:
				itemStatus.text = "Subscription not purchased";
				break;
			case SUBSCRIPTION_STATUS_ACTIVE:
				itemStatus.text = "Subscription active until";
				break;
			case SUBSCRIPTION_STATUS_EXPIRED:
				itemStatus.text = "Subscription expired at";
				break;
		}

		if (_itemInformation.expired_at != null && _itemInformation.status != SUBSCRIPTION_STATUS_NONE)
		{
			itemExpiration.text = UnixTimeToDateTime(_itemInformation.expired_at.Value).ToString();
			itemExpiration.gameObject.SetActive(true);
		}

		ChangeImageUrl(_itemInformation);
	}

	private void ChangeImageUrl(SubscriptionItem itemInformation)
	{
		if (!string.IsNullOrEmpty(_itemInformation.image_url))
			ImageLoader.Instance.GetImageAsync(_itemInformation.image_url, LoadImageCallback);
		else
		{
			loadingCircle.SetActive(false);
			itemImage.sprite = null;
		}
	}

	void LoadImageCallback(string url, Sprite image)
	{
		loadingCircle.SetActive(false);
		itemImage.sprite = image;
	}
	
	private DateTime UnixTimeToDateTime(long unixTime)
	{
		DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		dtDateTime = dtDateTime.AddSeconds(unixTime).ToLocalTime();
		return dtDateTime;
	}
}