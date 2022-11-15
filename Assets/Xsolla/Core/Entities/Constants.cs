namespace Xsolla.Core
{
	public static class Constants
	{
		public const string SdkVersion = "1.3.0";

		public const string DEFAULT_PROJECT_ID = "77640";
		public const string DEFAULT_LOGIN_ID = "026201e3-7e40-11ea-a85b-42010aa80004";
		public const int 	DEFAULT_OAUTH_CLIENT_ID = 57;
		public const string DEFAULT_REDIRECT_URL = "https://login.xsolla.com/api/blank";
		public const string DEFAULT_WEB_STORE_URL = "https://sitebuilder.xsolla.com/game/sdk-web-store/";

		public const string LAST_SUCCESS_AUTH_TOKEN = "xsolla_login_last_success_auth_token";
		public const string LAST_SUCCESS_OAUTH_REFRESH_TOKEN = "xsolla_login_last_success_oauth_refresh_token";

		public const string INVENTORY_TUTORIAL_COMPLETED = "xsolla_inventory_tutorial_completion_flag";
		public const string INVENTORY_TUTORIAL_HIGHLIGHT_TAG = "Highlight";

		public const string BASE_STORE_API_URL = "https://store.xsolla.com/api/v2/project/{0}";
	}
}