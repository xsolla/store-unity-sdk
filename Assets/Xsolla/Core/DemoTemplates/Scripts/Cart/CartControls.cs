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
	[SerializeField] private Text totalLabelText;
	[SerializeField] private Text subtotalLabelText;
	[SerializeField] private Text discountLabelText;
	[SerializeField] private GameObject loaderPrefab;

	private GameObject _loaderObject;

	public Action OnBuyCart
	{
		set
		{
			buyButton.onClick = value;
		}
	}

	public Action OnClearCart
	{
		set
		{
			clearCartButton.onClick = value;
		}
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

	public void Initialize(float totalPrice = 0f, float discount = 0f)
	{
		totalPriceText.text = PriceFormatter.FormatPrice(totalPrice);
		ShowTotal(totalPrice >= 0.01f);

		if (discount >= 0.01f)
			ShowDiscount(discount, totalPrice + discount);
		else
			HideDiscount();
	}

	private void ShowTotal(bool show)
	{
		totalLabelText.gameObject.SetActive(show);
		totalPriceText.gameObject.SetActive(show);
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
		var discountPriceValue = string.Format("-{0}", PriceFormatter.FormatPrice(discount));
		discountPriceText.text = discountPriceValue;
		subtotalPriceText.text = PriceFormatter.FormatPrice(subtotal);
	}
}
