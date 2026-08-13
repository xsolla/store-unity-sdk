using Xsolla.Core;

namespace Xsolla.TechDemo.Scenarios
{
	public class CompletesItemPurchaseByCustomTabsSandbox : SandboxPurchaseScenario
	{
		protected override string ItemSku => "booster_max_1";

		protected override PlatformSpecificAppearance Appearance
			=> new() {
				AndroidActivityType = AndroidActivityType.CustomTab
			};

		public override void SetUp()
		{
			base.SetUp();
			SetInAppBrowserActive(true);
		}
	}
}