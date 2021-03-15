using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassDescriptionDistributor : MonoBehaviour
    {
		[SerializeField] BattlePassDescriptionProvider DescriptionProvider = default;
		[SerializeField] BaseBattlePassDescriptionSubscriber[] DescriptionSubscribers = default;

		private void Awake()
		{
			DescriptionProvider.BattlePassDescriptionArrived += OnBattlePassDescriptionArrived;
		}

		private void OnBattlePassDescriptionArrived(BattlePassDescription battlePassDescription)
		{
			foreach (var subscriber in DescriptionSubscribers)
				subscriber.OnBattlePassDescriptionArrived(battlePassDescription);
		}

		private void Start()
		{
			DescriptionProvider.GetBattlePassDescription();
		}
	}
}
