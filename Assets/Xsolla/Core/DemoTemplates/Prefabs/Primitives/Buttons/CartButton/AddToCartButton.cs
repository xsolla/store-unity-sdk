using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AddToCartButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IDragHandler
{
	public static event Action OnCursorEnter;
	public static event Action OnCursorExit;

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
	
	public void OnDrag(PointerEventData eventData)
	{
	}
	
	public void OnPointerEnter(PointerEventData eventData)
	{
		OnCursorEnter?.Invoke();
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
		OnCursorExit?.Invoke();
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

			onClick?.Invoke(_isSelected);
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