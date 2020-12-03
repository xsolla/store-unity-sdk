using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class MenuButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler,
		IPointerUpHandler, IDragHandler
	{
		[SerializeField] Image image = default;

		[SerializeField] Text text = default;

		[SerializeField] Sprite normalStateSprite = default;
		[SerializeField] Sprite selectedStateSprite = default;
		[SerializeField] Sprite hoverStateSprite = default;
		[SerializeField] Sprite pressedStateSprite = default;

		[SerializeField] Color normalStateTextColor = default;
		[SerializeField] Color selectedStateTextColor = default;
		[SerializeField] Color hoverStateTextColor = default;
		[SerializeField] Color pressedStateTextColor = default;

		[SerializeField] bool capitalizeText = true;

		[SerializeField] bool isSelectable = true;

		bool _isClickInProgress;
		bool _isSelected;

		public static event Action OnCursorEnter;
		public static event Action OnCursorExit;

		public Action<string> onClick;
		private DateTime _lastClick;
		private float _rateLimitMs = StoreConstants.DEFAULT_BUTTON_RATE_LIMIT_MS;

		string _buttonId = "";

		void Awake()
		{
			_lastClick = DateTime.MinValue;
		}

		private void PerformClickEvent()
		{
			TimeSpan ts = DateTime.Now - _lastClick;
			if (ts.TotalMilliseconds > _rateLimitMs)
			{
				_lastClick += ts;
				onClick?.Invoke(_buttonId);
			}
		}

		public void Select(bool triggerClickEvent = true)
		{
			_isSelected = true;
			_isClickInProgress = false;

			if (triggerClickEvent)
			{
				PerformClickEvent();
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
			get { return text.text; }
			set { text.text = capitalizeText ? value.ToUpper() : value; }
		}

		public string Id
		{
			get { return _buttonId; }
			set { _buttonId = value; }
		}

		public bool IsSelected
		{
			get { return _isSelected && isSelectable; }
		}

		public void OnDrag(PointerEventData eventData)
		{
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			OnCursorEnter?.Invoke();

			if (!IsSelected)
			{
				OnHover();
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			OnCursorExit?.Invoke();

			if (IsSelected)
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

			if (!IsSelected)
			{
				OnPressed();
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (_isClickInProgress && !IsSelected)
			{
				_isSelected = true;

				OnSelected();

				PerformClickEvent();
			}

			_isClickInProgress = false;
		}

		protected void SetImageSprite(Image i, Sprite sprite)
		{
			if(i != null && sprite != null)
				i.sprite = sprite;
		}

		protected virtual void OnNormal()
		{
			SetImageSprite(image, normalStateSprite);
			text.color = normalStateTextColor;
		}

		protected virtual void OnHover()
		{
			SetImageSprite(image, hoverStateSprite);
			text.color = hoverStateTextColor;
		}

		protected virtual void OnPressed()
		{
			SetImageSprite(image, pressedStateSprite);
			text.color = pressedStateTextColor;
		}

		protected virtual void OnSelected()
		{
			SetImageSprite(image, selectedStateSprite);
			text.color = selectedStateTextColor;
		}
	}
}
