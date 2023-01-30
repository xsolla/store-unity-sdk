using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Demo
{
	internal class LoginWidgetAuth : LoginAuthorization
	{
		public override void TryAuth(params object[] args)
		{
			XsollaAuth.Instance.AuthWithXsollaWidget(
				onSuccess: token =>
				{
					BrowserHelper.Instance.Close();
					base.OnSuccess?.Invoke(token);
				});

			BrowserHelper.Instance.InAppBrowser.UpdateSize(820, 840);
		}
	}
}