using System;
using UnityEngine;
using UnityEngine.UI;

public class CartControls : MonoBehaviour
{
	[SerializeField] private SimpleTextButton buyButton;
	[SerializeField] private SimpleButton clearCartButton;
	
	[SerializeField] private Text totalPriceText;
	[SerializeField] private Text subtotalPriceText;
	[SerializeField] private Text discountPriceText;
	[SerializeField] private Text subtotalLabelText;
	[SerializeField] private Text discountLabelText;
	[SerializeField] private GameObject loaderPrefab;

	private GameObject _loaderObject;

	public Action OnBuyCart
	{
		set => buyButton.onClick = value;
	}
	public Action OnClearCart {
		set => clearCartButton.onClick = value;
	}

	private void Start()
	{
		HideDiscount();
	}

	public bool IsBuyButtonLocked()
	{
		return buyButton.IsLocked();
	}
	
	public void LockBuyButton()
	{
		buyButton.Lock();
		if (loaderPrefab != null) {
			_loaderObject = Instantiate(loaderPrefab, buyButton.transform);
		}
	}

	public void UnlockBuyButton()
	{
		buyButton.Unlock();
		
		if (_loaderObject == null) return;
		Destroy(_loaderObject);
		_loaderObject = null;
	}

	public void Initialize(float totalPrice, bool showDiscount = false, float discount = 0F)
	{
		totalPriceText.text = GetFormattedPrice(totalPrice);
		if(showDiscount)
			ShowDiscount(discount, totalPrice + discount);
		else
			HideDiscount();
	}

	private void HideDiscount()
	{
		subtotalLabelText.gameObject.SetActive(false);
		subtotalPriceText.gameObject.SetActive(false);
		discountLabelText.gameObject.SetActive(false);
		discountPriceText.gameObject.SetActive(false);
	}

	private void ShowDiscount(float discount, float subtotal)
	{
		subtotalLabelText.gameObject.SetActive(true);
		subtotalPriceText.gameObject.SetActive(true);
		discountLabelText.gameObject.SetActive(true);
		discountPriceText.gameObject.SetActive(true);
		discountPriceText.text = $"-{GetFormattedPrice(discount)}";
		subtotalPriceText.text = GetFormattedPrice(subtotal);
	}

	private string GetFormattedPrice(float price)
	{
		return $"${price:F2}";
	}
}