using System;

[Serializable]
public class ValidateToken
{
	public string token;

	public ValidateToken(string token)
	{
		this.token = token;
	}
}