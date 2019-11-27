using System;

[Serializable]
public class ResetPassword
{
	public string username;

	public ResetPassword(string username)
	{
		this.username = username;
	}
}