using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IDragHandler
{
	Image _image;
	
	[SerializeField]
	Text text;
	
	[SerializeField]
	Sprite normalStateSprite;
	[SerializeField]
	Sprite selectedStateSprite;
	[SerializeField]
	Sprite hoverStateSprite;
	[SerializeField]
	Sprite pressedStateSprite;
	
	[SerializeField]
	Color normalStateTextColor;
	[SerializeField]
	Color selectedStateTextColor;
	[SerializeField]
	Color hoverStateTextColor;
	[SerializeField]
	Color pressedStateTextColor;
	
	[SerializeField]
	bool capitalizeText = true;

	bool _isClickInProgress;
	bool _isSelected;

	public Action<string> onClick;

	string _buttonId;

	void Awake()
	{
		_image = GetComponent<Image>();
	}

	public void Select(bool triggerClickEvent = true)
	{
		_isSelected = true;
		_isClickInProgress = false;
		
		if (onClick != null && triggerClickEvent)
		{
			onClick.Invoke(Text);
		}
		
		OnSelected();
	}
	
	public void Deselect()
	{
		_isSelected = false;
		_isClickInProgress = false;
		
		OnNormal();
	}

	public string Text
	{
		get
		{
			return _buttonId;
		}
		set
		{
			_buttonId = value;

			text.text = capitalizeText ? _buttonId.ToUpper() : _buttonId;
		}
	}

	public bool IsSelected
	{
		get { return _isSelected; }
	}

	public void OnDrag(PointerEventData eventData)
	{
	}
	
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!_isSelected)
		{
			OnHover();
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

		if (!_isSelected)
		{
			OnPressed();
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (_isClickInProgress && !_isSelected)
		{
			_isSelected = true;
			
			OnSelected();
			
			if (onClick != null)
			{
				onClick.Invoke(Text);
			}
		}
		
		_isClickInProgress = false;
	}	
	
	protected virtual void OnNormal()
	{
		_image.sprite = normalStateSprite;
		text.color = normalStateTextColor;
	}
	
	protected virtual void OnHover()
	{
		_image.sprite = hoverStateSprite;
		text.color = hoverStateTextColor;
	}
	
	protected virtual void OnPressed()
	{
		_image.sprite = pressedStateSprite;
		text.color = pressedStateTextColor;
	}
	
	protected virtual void OnSelected()
    {
     	_image.sprite = selectedStateSprite;
     	text.color = selectedStateTextColor;
    }
}