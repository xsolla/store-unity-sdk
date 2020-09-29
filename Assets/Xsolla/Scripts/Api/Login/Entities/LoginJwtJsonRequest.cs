using System;

[Serializable]
public class LoginJwtJsonRequest
{
	public string username;
	public string password;
	public bool remember_me;

	public LoginJwtJsonRequest(string username, string password, bool rememberMe)
	{
		this.username = username;
		this.password = password;
		this.remember_me = rememberMe;
	}
}