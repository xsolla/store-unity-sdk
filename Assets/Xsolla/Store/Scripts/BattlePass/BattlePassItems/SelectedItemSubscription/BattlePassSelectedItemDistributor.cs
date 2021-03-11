using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassSelectedItemDistributor : MonoBehaviour
    {
		[SerializeField] private BattlePassItemsManager SelectedItemProvider = default;
		[SerializeField] private BaseBattlePassSelectedItemSubscriber[] SelectedItemSubscribers = default;

		private void Awake()
		{
			foreach (var subscriber in SelectedItemSubscribers)
				SelectedItemProvider.ItemSelected += subscriber.OnItemSelected;
		}
	}
}
