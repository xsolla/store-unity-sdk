namespace Xsolla.TechDemo.Scenarios
{
	public class CompletesItemPurchaseByExternalBrowserLive : LivePurchaseScenario
	{
		protected override string ItemSku => "gloves_1";

		public override void SetUp()
		{
			base.SetUp();
			SetInAppBrowserActive(false);
		}
	}
}