using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Bridge
{
	public class BridgeUI : MonoBehaviour
	{
		[field: SerializeField] private Button StartSocialAuthButton { get; set; }
		[field: SerializeField] private Button StartWidgetAuthButton { get; set; }

		[field: Space]
		[field: SerializeField] private BridgeTest BridgeTest { get; set; }
		[field: SerializeField] private SocialAuthTest SocialAuthTest { get; set; }
		[field: SerializeField] private WidgetAuthTest WidgetAuthTest { get; set; }

		private void Start()
		{
			BridgeTest.Execute();

			StartSocialAuthButton
				.onClick
				.AddListener(() => SocialAuthTest.Execute());

			StartWidgetAuthButton
				.onClick
				.AddListener(() => WidgetAuthTest.Execute());
		}
	}
}