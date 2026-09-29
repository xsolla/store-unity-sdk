using Xsolla.Core;

namespace Xsolla.TechDemo.Scenarios
{
	public class CompletesItemPurchaseByTwaSandbox : SandboxPurchaseScenario
	{
		protected override string ItemSku => "booster_max_1";

		protected override PlatformSpecificAppearance Appearance
			=> new() {
				AndroidActivityType = AndroidActivityType.TrustedWebActivity
			};

		public override void SetUp()
		{
			base.SetUp();
			SetInAppBrowserActive(true);
		}
	}
}