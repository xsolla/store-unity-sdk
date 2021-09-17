namespace Xsolla.Demo
{
	public class LauncherAuth : LoginAuthorization
	{
		public override void TryAuth(params object[] args)
		{
			string launcherToken = LauncherArguments.Instance.GetToken();

			if (!string.IsNullOrEmpty(launcherToken))
			{
				Debug.Log("LauncherAuth.TryAuth: Token loaded");
				base.OnSuccess?.Invoke(launcherToken);
			}
			else
			{
				Debug.Log("LauncherAuth.TryAuth: No token");
				base.OnError?.Invoke(null);
			}
		}
	}
}
