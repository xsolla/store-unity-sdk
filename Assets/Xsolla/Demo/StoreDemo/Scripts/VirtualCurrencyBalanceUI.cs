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
		StoreController _storeController = FindObjectOfType<StoreController>();
		if(_storeController != null) {
			_storeController.GetImageAsync(item.image_url, LoadImageHandler);
		} else {
			Debug.LogWarning("StoreController is missing!");
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
