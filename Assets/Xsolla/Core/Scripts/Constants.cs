namespace Xsolla.Core
{
	public static partial class Constants
	{
		public const string LoginSdkVersion = "0.6.2.0";
		public const string StoreSdkVersion = "0.6.2.0";

		public const string DEFAULT_PROJECT_ID = "44056";
		public const string DEFAULT_LOGIN_ID = "e6dfaac6-78a8-11e9-9244-42010aa80004";
		public const string DEFAULT_PLATFORM_LOGIN_ID = "5c5d2ca4-5fa6-11ea-b687-42010aa80004";

		public const string LAST_SUCCESS_AUTH_TOKEN = "xsolla_login_last_success_auth_token";
		public const string LAST_SUCCESS_OAUTH_REFRESH_TOKEN = "xsolla_login_last_success_oauth_refresh_token";
		public const string OAUTH_REFRESH_TOKEN_EXPIRATION_TIME = "xsolla_login_oauth_refresh_token_expiration_time";

		public const string CartGroupName = "CART";
		public const string CurrencyGroupName = "CURRENCY";
		public const string UngroupedGroupName = "UNGROUPED";
		public const string InventoryContainerName = "INVENTORY";
		public const string SubscriptionsContainerName = "SUBSCRIPTIONS";
		public const string AttributesContainerName = "ATTRIBUTES";
		public const string EmptyContainerName = "EMPTY_CONTAINER";

		public const float DefaultButtonRateLimitMs = 500.0F;
		public const float LoginPageRateLimitMs = 1500.0F;
	}
}