using System;

namespace Xsolla.Core
{
	[Serializable]
	public class LoginError
	{
		[Serializable]
		public class Data
		{
			public string code;
			public string description;
		}

		public Data error;

		public Error ToError()
		{
			if (this.error == null)
				return null;
			else
				return new Error() { statusCode = error.code, errorCode = error.code, errorMessage = error.description };
		}
	}
}
