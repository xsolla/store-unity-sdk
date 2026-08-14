using Xsolla.Core;

namespace Xsolla.TechDemo.Scenarios
{
	public class CompletesItemPurchaseByWebViewSandbox : SandboxPurchaseScenario
	{
		protected override string ItemSku => "booster_max_1";

		protected override PlatformSpecificAppearance Appearance
			=> new() {
				AndroidActivityType = AndroidActivityType.WebView
			};

		public override void SetUp()
		{
			base.SetUp();
			SetInAppBrowserActive(true);
		}
	}
}