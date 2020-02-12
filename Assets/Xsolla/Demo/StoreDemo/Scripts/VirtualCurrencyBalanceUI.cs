using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Store;

public class VirtualCurrencyBalanceUI : MonoBehaviour
{
	[SerializeField]
	public Image Image;
	[SerializeField]
	public Text Text;

	public void Initialize(StoreItem item)
	{
		ImageLoader _imageLoader = FindObjectOfType<ImageLoader>();
		if(_imageLoader != null) {
			_imageLoader.GetImageAsync(item.image_url, LoadImageHandler);
		} else {
			Debug.LogWarning("ImageLoader is missing!");
		}
	}

	public void SetBalance(uint balance)
	{
		if (Text)
			Text.text = balance.ToString();
	}

	void LoadImageHandler(string url, Sprite sprite)
	{
		if (Image != null)
			Image.sprite = sprite;
	}
}
