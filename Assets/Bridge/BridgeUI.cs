using UnityEngine;
using UnityEngine.UI;
using Xsolla.Catalog;
using Xsolla.Orders;

namespace Xsolla.Bridge
{
	public class BridgeUI : MonoBehaviour
	{
		[field: SerializeField] private Button StartSocialAuthButton { get; set; }
		[field: SerializeField] private Button StartWidgetAuthButton { get; set; }
		[field: SerializeField] private Button StartPaymentsButton { get; set; }

		[field: Space]
		[field: SerializeField] private BridgeTest BridgeTest { get; set; }
		[field: SerializeField] private SocialAuthTest SocialAuthTest { get; set; }
		[field: SerializeField] private WidgetAuthTest WidgetAuthTest { get; set; }
		[field: SerializeField] private PaymentsTest PaymentsTest { get; set; }

		private void Start()
		{
			BridgeTest.Execute();

			StartSocialAuthButton
				.onClick
				.AddListener(() => SocialAuthTest.Execute());

			StartWidgetAuthButton
				.onClick
				.AddListener(() => WidgetAuthTest.Execute());

			StartPaymentsButton
				.onClick
				.AddListener(LaunchPaymentsTest);
		}

		private void LaunchPaymentsTest()
		{
			XsollaCatalog.CreateOrder(
				"booster_min_1",
				orderData => PaymentsTest.Execute(orderData.token), 
				error => Debug.LogError(error.errorMessage));
		}
	}
}