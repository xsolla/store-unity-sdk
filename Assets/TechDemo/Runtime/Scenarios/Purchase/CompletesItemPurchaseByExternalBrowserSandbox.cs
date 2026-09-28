namespace Xsolla.TechDemo.Scenarios
{
	public class CompletesItemPurchaseByExternalBrowserSandbox : SandboxPurchaseScenario
	{
		protected override string ItemSku => "booster_max_1";

		public override void SetUp()
		{
			base.SetUp();
			SetInAppBrowserActive(false);
		}
	}
}