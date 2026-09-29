namespace Xsolla.TechDemo.Scenarios
{
	public class CompletesItemPurchaseByInternalBrowserSandbox : SandboxPurchaseScenario
	{
		protected override string ItemSku => "booster_max_1";
		protected override bool ClosesInAppBrowserAfterPurchase => true;

		public override void SetUp()
		{
			base.SetUp();
			SetInAppBrowserActive(true);
		}
	}
}