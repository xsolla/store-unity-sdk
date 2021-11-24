using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class MenuButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler,
		IPointerUpHandler
	{
		[SerializeField] Image image;

		[SerializeField] Text text;

		[SerializeField] Sprite normalStateSprite;
		[SerializeField] Sprite selectedStateSprite;
		[SerializeField] Sprite hoverStateSprite;
		[SerializeField] Sprite pressedStateSprite;
		
		[SerializeField] bool capitalizeText = true;

		[SerializeField] bool isSelectable = true;
		//[SerializeField] private DecoratorPointerEvents DecoratorPointerEvents;
		[SerializeField] private Text SelectedText;
		[SerializeField] private Color NormalTextColor;
		[SerializeField] private Color SelectedTextColor;
		[SerializeField] private GameObject OnSelectedObject;

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
				if (onClick != null)
					onClick.Invoke(_buttonId);
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

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (OnCursorEnter != null)
				OnCursorEnter.Invoke();

			if (!IsSelected)
			{
				OnHover();
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (OnCursorExit != null)
				OnCursorExit.Invoke();

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
			//DecoratorPointerEvents.IsMute = false;
			StartCoroutine(DoChangeColorOnNextFrame(NormalTextColor));

			if (OnSelectedObject)
				OnSelectedObject.SetActive(false);
		}

		protected virtual void OnHover()
		{
			SetImageSprite(image, hoverStateSprite);
		}

		protected virtual void OnPressed()
		{
			SetImageSprite(image, pressedStateSprite);
		}

		protected virtual void OnSelected()
		{
			SetImageSprite(image, selectedStateSprite);
			//DecoratorPointerEvents.IsMute = false;
			StartCoroutine(DoChangeColorOnNextFrame(SelectedTextColor));

			if (OnSelectedObject)
				OnSelectedObject.SetActive(true);
		}

		private IEnumerator DoChangeColorOnNextFrame(Color color)
		{
			yield return null;
			//DecoratorPointerEvents.IsMute = false;
			SelectedText.color = color;
		}
	}
}
