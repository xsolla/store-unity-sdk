using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassDescriptionDistributor : MonoBehaviour
    {
		[SerializeField] BattlePassDescriptionProvider DescriptionProvider;
		[SerializeField] BaseBattlePassDescriptionSubscriber[] DescriptionSubscribers;

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
