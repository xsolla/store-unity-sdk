using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Xsolla.Core;

public class SimpleButton : MonoBehaviour, ISimpleButton
{
	[SerializeField] Image image;
	[SerializeField] Sprite normalStateSprite;
	[SerializeField] Sprite hoverStateSprite;
	[SerializeField] Sprite pressedStateSprite;

	bool _isClickInProgress;

	public Action onClick;
	private DateTime lastClick;

	private float _rateLimitMs = 500.0F;
	public float RateLimitMs
	{
		get
		{
			return _rateLimitMs;
		}
		set
		{
			_rateLimitMs = value;
		}
	}

	public static event Action OnCursorEnter;
	public static event Action OnCursorExit;

	void Awake()
	{
		lastClick = DateTime.MinValue;
	}

	private void OnDisable()
	{
		OnNormal();
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

	private void PerformClickEvent()
	{
		TimeSpan ts = DateTime.Now - lastClick;
		if (ts.TotalMilliseconds > RateLimitMs)
		{
			lastClick += ts;
			if (onClick != null)
				onClick.Invoke();
		}
	}

	public virtual void OnPointerUp(PointerEventData eventData)
	{
		if (_isClickInProgress)
		{
			OnHover();
			PerformClickEvent();
		}

		_isClickInProgress = false;
	}
	
	protected void SetImageSprite(Image i, Sprite sprite)
	{
		if(i != null && sprite != null)
			i.sprite = sprite;
	}

	protected void SetImageSprite(Sprite sprite)
	{
		SetImageSprite(image, sprite);
	}

	protected virtual void OnNormal()
	{
		SetImageSprite(image, normalStateSprite);
		RaiseOnCursorExit();
	}

	protected virtual void OnHover()
	{
		SetImageSprite(image, hoverStateSprite);
		RaiseOnCursorEnter();
	}

	protected virtual void OnPressed()
	{
		SetImageSprite(image, pressedStateSprite);
	}

	protected void RaiseOnCursorEnter()
	{
		if (OnCursorEnter != null)
			OnCursorEnter.Invoke();
	}

	protected void RaiseOnCursorExit()
	{
		if (OnCursorExit != null)
			OnCursorExit.Invoke();
	}
}
