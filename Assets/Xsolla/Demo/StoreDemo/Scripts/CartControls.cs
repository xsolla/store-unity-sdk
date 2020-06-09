using System;
using UnityEngine;
using UnityEngine.UI;

public class CartControls : MonoBehaviour
{
	[SerializeField]
	SimpleTextButton buyButton;
	
	[SerializeField]
	SimpleButton clearCartButton;
	
	[SerializeField]
	Text priceText;
	
	[SerializeField]
	GameObject LoaderPrefab;

	private GameObject _loaderObject;

	public Action OnBuyCart
	{
		set => buyButton.onClick = value;
	}
	public Action OnClearCart {
		set => clearCartButton.onClick = value;
	}

	public bool IsBuyButtonLocked()
	{
		return buyButton.IsLocked();
	}
	
	public void LockBuyButton()
	{
		buyButton.Lock();
		if (LoaderPrefab != null) {
			_loaderObject = Instantiate(LoaderPrefab, buyButton.transform);
		}
	}

	public void UnlockBuyButton()
	{
		buyButton.Unlock();
		
		if (_loaderObject == null) return;
		Destroy(_loaderObject);
		_loaderObject = null;
	}

	public void Initialize(float price)
	{
		priceText.text = "$" + price.ToString("F2");
	}
}