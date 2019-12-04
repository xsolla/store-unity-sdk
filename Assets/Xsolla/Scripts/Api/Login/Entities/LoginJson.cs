using System;

[Serializable]
public class LoginJson
{
	public string username;
	public string password;
	public bool remember_me;

	public LoginJson(string userName, string password, bool rememberMe)
	{
		username = userName;
		this.password = password;
		remember_me = rememberMe;
	}
}