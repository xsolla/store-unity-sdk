namespace Xsolla.TechDemo.Scenarios
{
	public class CompletesItemPurchaseByInternalBrowserLive : LivePurchaseScenario
	{
		protected override string ItemSku => "gloves_1";
		protected override bool ClosesInAppBrowserAfterPurchase => true;

		public override void SetUp()
		{
			base.SetUp();
			SetInAppBrowserActive(true);
		}
	}
}