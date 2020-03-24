namespace Xsolla.Core
{
	public static class Constants
	{
		public const string LoginSdkVersion = "0.5.0.1";
		public const string StoreSdkVersion = "0.5.0.1";
		
		public const string XsollaLoginToken = "Xsolla_Token";
		public const string XsollaLoginTokenExp = "Xsolla_Token_Exp";
		public const string UserLogin = "Xsolla_User_Login";
		public const string UserPassword = "Xsolla_User_Password";
		public const string UserShadowAccount = "Xsolla_User_ShadowAccount";
		public const string UserShadowPlatform = "Xsolla_User_ShadowPlatform";

		public const string XsollaStoreToken = "Xsolla_Store_Token";

		public const string CartGroupName = "CART";
		public const string CurrencyGroupName = "CURRENCY";
		public const string UngroupedGroupName = "UNGROUPED";
		public const string InventoryContainerName = "INVENTORY";
		public const string AttributesContainerName = "ATTRIBUTES";
		public const string EmptyContainerName = "EMPTY_CONTAINER";

		public const float DefaultButtonRateLimitMs = 500.0F;
		public const float LoginPageRateLimitMs = 1500.0F;
	}
}