using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassSelectedItemDistributor : MonoBehaviour
    {
		[SerializeField] private BattlePassItemsManager SelectedItemProvider = default;
		[SerializeField] private BaseBattlePassSelectedItemSubscriber[] SelectedItemSubscribers = default;

		private void Awake()
		{
			SelectedItemProvider.ItemSelected += OnItemSelected;
		}

		private void OnItemSelected(ItemSelectedEventArgs selectedItemEventArgs)
		{
			foreach (var subscriber in SelectedItemSubscribers)
				subscriber.OnItemSelected(selectedItemEventArgs);
		}
	}
}
