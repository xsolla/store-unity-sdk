using Xsolla.Core;

namespace Xsolla.TechDemo.Scenarios
{
	public class CompletesItemPurchaseByWebViewLive : LivePurchaseScenario
	{
		protected override string ItemSku => "gloves_1";

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