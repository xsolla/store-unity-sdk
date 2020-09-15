using Xsolla.Login;

public static class UserInfoContainer
{
	public static UserInfo UserInfo { get; set; }

	private static string _lastEmail;
	public static string LastEmail
	{
		get
		{
			return UserInfo?.email ?? _lastEmail;
		}

		set => _lastEmail = value;
	}
}
