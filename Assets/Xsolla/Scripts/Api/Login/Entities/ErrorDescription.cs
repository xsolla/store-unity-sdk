using System;

namespace Xsolla.Login
{
	[Serializable]
	public class ErrorDescription
	{
		public string code;
		public string description;
		public Error error;

		public ErrorDescription(string _code, string _description, Error _error)
		{
			code = _code;
			description = _description;
			error = _error;
		}

		public override string ToString()
		{
			return string.Format("Code: {0}. Description: {1}", code, error);
		}
	}
}