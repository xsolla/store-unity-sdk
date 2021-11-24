using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class SocialProviderButton : MonoBehaviour, IPointerClickHandler
	{
		[SerializeField] private SocialProvider _socialProvider;

		public SocialProvider SocialProvider
		{
			get
			{
				return _socialProvider;
			}
		}

		public Action OnClick { get; set; }

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			if (OnClick != null)
				OnClick.Invoke();
		}
	}
}
