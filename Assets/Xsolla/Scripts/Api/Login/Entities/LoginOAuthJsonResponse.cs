using System;

[Serializable]
public class LoginOAuthJsonResponse
{
	public string access_token;
	public int expires_in;
	public string refresh_token;
	public string scope;
	public string token_type;
}