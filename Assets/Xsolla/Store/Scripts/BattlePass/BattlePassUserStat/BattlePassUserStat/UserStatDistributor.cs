using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Demo
{
    public class UserStatDistributor : MonoBehaviour
    {
		[SerializeField] private BaseUserStatManager UserStatProvider = default;
		[SerializeField] private BaseUserStatSubscriber[] UserStatSubscribers = default;

		private void Awake()
		{
			foreach (var subscriber in UserStatSubscribers)
				UserStatProvider.UserStatArrived += subscriber.OnUserStatArrived;
		}
	}
}
