using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassPopupController : MonoBehaviour
    {
		[SerializeField] private GameObject RewardPopupPrefab = default;

		private Canvas _pageCanvas;

		private Canvas PageCanvas
		{
			get
			{
				if (!_pageCanvas)
					_pageCanvas = GameObject.FindObjectOfType<Canvas>();

				return _pageCanvas;
			}
			set { _pageCanvas = value; }
		}

		public void ShowRewards(BattlePassItemDescription[] battlePassItemDescriptions)
		{
			var gameObject = Instantiate(RewardPopupPrefab, PageCanvas.transform);
			var popupController = gameObject.GetComponent<BattlePassRewardPopup>();
			popupController.Initialize(battlePassItemDescriptions);
		}
	}
}
