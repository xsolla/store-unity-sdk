using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassRewardPopup : MonoBehaviour
    {
		[SerializeField] private SimpleButton[] CloseButtons = default;
		[SerializeField] private GameObject RewardItemPrefab = default;
		[SerializeField] private Transform RewardItemsRoot = default;

		private void Awake()
		{
			foreach (var button in CloseButtons)
				button.onClick += () => Destroy(this.gameObject);
		}

		public void Initialize(BattlePassItemDescription[] battlePassItemDescriptions)
		{
			foreach (var item in battlePassItemDescriptions)
			{
				var gameObject = Instantiate(RewardItemPrefab, RewardItemsRoot);
				var itemScript = gameObject.GetComponent<ItemUI>();
				itemScript.Initialize(item.ItemCatalogModel, item.Quantity);
			}
		}
	}
}
