using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SimpleButton : MonoBehaviour, ISimpleButton
{
	Image _image;
	
	[SerializeField]
	Sprite normalStateSprite;
	[SerializeField]
	Sprite hoverStateSprite;
	[SerializeField]
	Sprite pressedStateSprite;

	bool _isClickInProgress;

	public Action onClick;
	
	void Awake()
	{
		_image = GetComponent<Image>();
	}
	
	public virtual void OnDrag(PointerEventData eventData)
	{
	}
	
	public virtual void OnPointerEnter(PointerEventData eventData)
	{
		OnHover();
	}

	public virtual void OnPointerExit(PointerEventData eventData)
	{
		OnNormal();

		_isClickInProgress = false;
	}

	public virtual void OnPointerDown(PointerEventData eventData)
	{
		_isClickInProgress = true;

		OnPressed();
	}

	public virtual void OnPointerUp(PointerEventData eventData)
	{
		if (_isClickInProgress)
		{
			OnHover();
			
			if (onClick != null)
			{
				onClick.Invoke();
			}
		}
		
		_isClickInProgress = false;
	}	
	
	protected virtual void OnNormal()
	{
		_image.sprite = normalStateSprite;
	}
	
	protected virtual void OnHover()
	{
		_image.sprite = hoverStateSprite;
	}
	
	protected virtual void OnPressed()
	{
		_image.sprite = pressedStateSprite;
	}
}