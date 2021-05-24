using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassUserStatDistributor : MonoBehaviour
    {
		[SerializeField] private BaseBattlePassUserStatManager UserStatProvider = default;
		[SerializeField] private BaseBattlePassUserStatSubscriber[] UserStatSubscribers = default;

		private void Awake()
		{
			UserStatProvider.UserStatArrived += OnUserStatArrived;
		}

		private void OnUserStatArrived(BattlePassUserStat userStat)
		{
			foreach (var subscriber in UserStatSubscribers)
				subscriber.OnUserStatArrived(userStat);
		}
	}
}
