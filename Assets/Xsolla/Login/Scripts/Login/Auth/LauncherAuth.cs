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
				if (base.OnSuccess != null)
					base.OnSuccess.Invoke(launcherToken);
			}
			else
			{
				Debug.Log("LauncherAuth.TryAuth: No token");
				if (base.OnError != null)
					base.OnError.Invoke(null);
			}
		}
	}
}
