using System;

[Serializable]
public class RegistrationJson
{
	public string username;
	public string password;
	public string email;

	public RegistrationJson(string userName, string password, string email)
	{
		username = userName;
		this.password = password;
		this.email = email;
	}
}
