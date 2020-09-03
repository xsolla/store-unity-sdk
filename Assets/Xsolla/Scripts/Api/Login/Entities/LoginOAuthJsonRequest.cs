using System;

[Serializable]
public class LoginOAuthJsonRequest
{
	public string username;
	public string password;

	public LoginOAuthJsonRequest(string username, string password)
	{
		this.username = username;
		this.password = password;
	}
}