using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class BattlePassItemsInitialScroller : MonoBehaviour
    {
		[SerializeField] private ScrollRect ItemsScrollRect;
		[SerializeField] private BattlePassScrollPositionProvider PositionProvider;

		private bool _firstCall = true;

		private void Awake()
		{
			PositionProvider.ScrollPositionDefined += ScrollTo;
		}

		private void ScrollTo(int itemIndex, Vector2 targetScroll)
		{
			if (!_firstCall)
				return;
			else	
				_firstCall = false;

			if (itemIndex != 0)
				ItemsScrollRect.content.anchoredPosition = targetScroll;
		}
	}
}
