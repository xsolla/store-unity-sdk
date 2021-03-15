using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class BattlePassItemsInitialScroller : BaseBattlePassUserStatSubscriber
    {
		[SerializeField] private ScrollRect ItemsScrollRect = default;
		[SerializeField] private RectTransform ItemsContentRoot = default;
		[SerializeField] private float Padding = default;

		public bool IsInitialStateSet { get; private set; }

		public override void OnUserStatArrived(BattlePassUserStat userStat)
		{
			IsInitialStateSet = false;

			var itemBlockIndex = userStat.Level - 1;
			
			if (itemBlockIndex != 0)
			{
				var itemBlockToShow = (RectTransform)ItemsContentRoot.GetChild(itemBlockIndex);
				ScrollTo(itemBlockToShow, Padding);
			}

			IsInitialStateSet = true;
		}

		private void ScrollTo(RectTransform item, float padding = 0)
		{
			float viewportWidth = ItemsScrollRect.viewport.rect.width;
			Vector2 scrollPosition = ItemsScrollRect.content.anchoredPosition;

			float elementLeft = item.anchoredPosition.x;
			float elementRight = elementLeft + item.rect.width;

			float visibleContentTop = -scrollPosition.x - padding;
			float visibleContentBottom = -scrollPosition.x - viewportWidth + padding;

			float scrollDelta =
				elementLeft > visibleContentTop ? visibleContentTop - elementLeft :
				elementRight < visibleContentBottom ? visibleContentBottom - elementRight :
				0f;

			scrollPosition.x += scrollDelta;
			ItemsScrollRect.content.anchoredPosition = scrollPosition;
		}
	}
}
