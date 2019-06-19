using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AddToCartButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
	Image _image;

	[SerializeField]
	Sprite normalStateSprite;
	[SerializeField]
	Sprite selectedStateSprite;
	[SerializeField]
	Sprite hoverUnselectedStateSprite;
	[SerializeField]
	Sprite hoverSelectedStateSprite;
	[SerializeField]
	Sprite pressedStateSprite;

	bool _isClickInProgress;
	
	bool _isSelected;

	public Action<bool> onClick;
	
	void Awake()
	{
		_image = GetComponent<Image>();
	}

	public void Select(bool bSelect)
	{
		if (bSelect)
		{
			OnSelected();
		}
		else
		{
			OnNormal();
		}

		_isSelected = bSelect;
	}
	
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!_isSelected)
		{
			OnHoverUnselected();
		}
		else
		{
			OnHoverSelected();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (_isSelected)
		{
			OnSelected();
		}
		else
		{
			OnNormal();
		}

		_isClickInProgress = false;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		_isClickInProgress = true;

		OnPressed();
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (_isClickInProgress)
		{
			_isSelected = !_isSelected;

			if (_isSelected)
			{
				OnSelected();
			}
			else
			{
				OnHoverUnselected();
			}
			
			if (onClick != null)
			{
				onClick.Invoke(_isSelected);
			}
		}
		
		_isClickInProgress = false;
	}	
	
	protected virtual void OnNormal()
	{
		_image.sprite = normalStateSprite;
	}
	
	protected virtual void OnHoverUnselected()
	{
		_image.sprite = hoverUnselectedStateSprite;
	}
	
	protected virtual void OnHoverSelected()
	{
		_image.sprite = hoverSelectedStateSprite;
	}
	
	protected virtual void OnPressed()
	{
		_image.sprite = pressedStateSprite;
	}
	
	protected virtual void OnSelected()
	{
		_image.sprite = selectedStateSprite;
	}
}