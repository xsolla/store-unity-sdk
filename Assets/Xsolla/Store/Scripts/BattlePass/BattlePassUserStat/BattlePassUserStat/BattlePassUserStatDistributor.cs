using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassUserStatDistributor : MonoBehaviour
    {
		[SerializeField] private BaseBattlePassUserStatManager UserStatProvider = default;
		[SerializeField] private BaseBattlePassUserStatSubscriber[] UserStatSubscribers = default;

		private void Awake()
		{
			foreach (var subscriber in UserStatSubscribers)
				UserStatProvider.UserStatArrived += subscriber.OnUserStatArrived;
		}
	}
}
