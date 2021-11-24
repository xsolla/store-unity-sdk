using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassPopupFactory : MonoBehaviour
    {
		[SerializeField] private GameObject RewardPopupPrefab;
		[SerializeField] private GameObject LevelUpPopupPrefab;
		[SerializeField] private GameObject BuyPremiumPopupPrefab;
		[SerializeField] private GameObject BuyPremiumSuccessPopupPrefab;
		[SerializeField] private GameObject BattlePassExpiredPopupPrefab;

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

		public void CreateRewardsPopup(BattlePassItemDescription[] battlePassItemDescriptions)
		{
			var popupController = CreatePopup<BattlePassRewardPopup>(RewardPopupPrefab);
			popupController.Initialize(battlePassItemDescriptions);
		}

		public BattlePassLevelUpPopup CreateLevelUpPopup()
		{
			return CreatePopup<BattlePassLevelUpPopup>(LevelUpPopupPrefab);
		}

		public BattlePassBuyPremiumPopup CreateBuyPremiumPopup()
		{
			return CreatePopup<BattlePassBuyPremiumPopup>(BuyPremiumPopupPrefab);
		}

		public void CreateBuyPremiumSuccessPopup()
		{
			CreatePopup<GameObject>(BuyPremiumSuccessPopupPrefab);
		}

		public void CreateBattlePassExpiredPopup()
		{
			CreatePopup<GameObject>(BattlePassExpiredPopupPrefab);
		}

		private T CreatePopup<T>(GameObject popupPrefab)
		{
			var popupObject = Instantiate(popupPrefab, PageCanvas.transform);
			var popupController = popupObject.GetComponent<T>();
			return popupController;
		}
	}
}
