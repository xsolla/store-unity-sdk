using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class BattlePassScrollPositionProvider : BaseBattlePassUserStatSubscriber
	{
		[SerializeField] private ScrollRect ItemsScrollRect;
		[SerializeField] private RectTransform ItemsContentRoot;
		[SerializeField] private float Padding;

		public event Action<int, Vector2> ScrollPositionDefined;

		public override void OnUserStatArrived(BattlePassUserStat userStat)
		{
			var itemBlockIndex = userStat.Level - 1;
			var itemBlockToShow = (RectTransform)ItemsContentRoot.GetChild(itemBlockIndex);
			var targetScrollPositon = GetTargetScrollPosition(itemBlockToShow, Padding);
			if (ScrollPositionDefined != null)
				ScrollPositionDefined.Invoke(itemBlockIndex, targetScrollPositon);
		}

		private Vector2 GetTargetScrollPosition(RectTransform item, float padding = 0)
		{
			float viewportWidth = ItemsScrollRect.viewport.rect.width;
			Vector2 scrollPosition = ItemsScrollRect.content.anchoredPosition;

			float elementLeft = item.anchoredPosition.x;
			float elementRight = elementLeft + item.rect.width;

			float visibleContentLeft = -scrollPosition.x - padding;
			float visibleContentRight = -scrollPosition.x - viewportWidth + padding;

			float scrollDelta =
				elementLeft > visibleContentLeft ? visibleContentLeft - elementLeft :
				elementRight < visibleContentRight ? visibleContentRight - elementRight :
				0f;

			scrollPosition.x += scrollDelta;
			return scrollPosition;
		}
	}
}
