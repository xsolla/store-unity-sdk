using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassPageContentController : MonoBehaviour
    {
		[SerializeField] BattlePassDescriptionProvider DescriptionProvider = default;
		[SerializeField] BaseBattlePassDescriptionSubscriber[] DescriptionSubscribers = default;

		private void Awake()
		{
			foreach (var subscriber in DescriptionSubscribers)
				DescriptionProvider.BattlePassDescriptionArrived += subscriber.OnBattlePassDescriptionArrived;
		}

		private void Start()
		{
			DescriptionProvider.GetBattlePassDescription();
		}
	}
}
