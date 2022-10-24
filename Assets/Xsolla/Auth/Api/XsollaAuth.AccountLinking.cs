using System;
using Xsolla.Core;

namespace Xsolla.Auth
{
	public partial class XsollaAuth : MonoSingleton<XsollaAuth>
	{
		private const string URL_USER_CONSOLE_AUTH = "https://livedemo.xsolla.com/sdk/sdk-shadow-account/auth";

		#region Comment
		/// <summary>
		/// This method is used for authenticating users in Xsolla Login,
		/// who play on the consoles and other platforms
		/// where Xsolla Login isn't used. You must implement it
		/// on your server side.
		/// Integration flow on the server side:
		/// <list type="number">
		///		<item>
		///			<term>Generate server JWT</term>
		///			<description>
		///				<list type="bullet">
		///					<item>
		///						<term>Request credentionals</term>
		///						<description>before write any code, contact with support by email:<see cref="support@xsolla.com"/>
		///						and request <c>ClientID</c> + <c>ClientSecret</c>.
		///						</description>
		///					</item>
		///					<item>
		///						<term>Implement method: </term>
		///						<description>
		///							<see cref="https://developers.xsolla.com/login-api/oauth-20/generate-user-jwt"/>
		///							with application/x-www-form-urlencoded payload parameters:
		///							<list type="bullet">
		///								<item>
		///									<description>client_id=YOUR_CLIENT_ID</description>
		///								</item>
		///								<item>
		///									<description>client_secret=YOUR_CLIENT_SECRET</description>
		///								</item>
		///								<item>
		///									<description>grant_type=client_credentials</description>
		///								</item>
		///							</list>
		///						</description>
		///					</item>
		///				</list>
		///			</description>
		///		</item>
		///		<item>
		///			<term>Implement auth method</term>
		///			<description>
		///				<see cref="https://developers.xsolla.com/login-api/jwt/auth-by-custom-id"/>
		///				with:
		///				<list type="bullet">
		///					<item>
		///						<term>Query parameters</term>
		///						<description><c>?publisher_project_id=XsollaSettings.StoreProjectId</c></description>
		///					</item>
		///					<item>
		///						<term>Headers</term>
		///						<description>
		///						`Content-Type: application/json` and `X-SERVER-AUTHORIZATION: YourGeneratedJwt`
		///						</description>
		///					</item>
		///				</list>
		///				
		///			</description>
		///		</item>
		/// </list>
		/// </summary>
		/// <param name="userId">Social platform (XBox, PS4, etc) user unique identifier.</param>
		/// <param name="platform">Platform name (XBox, PS4, etc).</param>
		/// <param name="successCase">Success operation callback.</param>
		/// <param name="failedCase">Failed operation callback.</param>
		#endregion
		public void SignInConsoleAccount(string userId, string platform, Action<string> successCase, Action<Error> failedCase)
		{
			var with_logout = XsollaSettings.InvalidateExistingSessions ? "1" : "0";
			var url = $"{URL_USER_CONSOLE_AUTH}?user_id={userId}&platform={platform}&with_logout={with_logout}";
			WebRequestHelper.Instance.GetRequest(SdkType.Login, url, (TokenEntity result) => { successCase?.Invoke(result.token); }, failedCase);
		}
	}
}
