using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Bridge
{
	public class BridgeUI : MonoBehaviour
	{
		[field: SerializeField] private Button StartSocialAuthButton { get; set; }
		[field: SerializeField] private BridgeTest BridgeTest { get; set; }
		[field: SerializeField] private SocialAuthTest SocialAuthTest { get; set; }

		private void Start()
		{
			BridgeTest.Execute();

			StartSocialAuthButton
				.onClick
				.AddListener(() => SocialAuthTest.Execute());
		}
	}
}