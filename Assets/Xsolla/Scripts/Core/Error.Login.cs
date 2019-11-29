using System;
using System.Collections.Generic;

namespace Xsolla.Core
{
	public partial class Error
	{
		[Serializable]
		public class Login
		{
			[Serializable]
			public class Data
			{
				public uint code;
				public string description;
			}
			public Data error;

			public Error ToStoreError()
			{
				Error storeError = new Error();
				if (error == null)
					return null;
				storeError.statusCode = storeError.errorCode = error.code.ToString();
				storeError.errorMessage = error.description;
				return storeError;
			}
		}

		public static readonly Dictionary<string, ErrorType> LoginErrors =
			new Dictionary<string, ErrorType>()
			{
				{"003-007", ErrorType.UserIsNotActivated},
				{"010-007", ErrorType.CaptchaRequiredException},
			};

		public static readonly Dictionary<string, ErrorType> RegistrationErrors =
			new Dictionary<string, ErrorType>()
			{
				{"010-003", ErrorType.RegistrationNotAllowedException},
				{"003-003", ErrorType.UsernameIsTaken},
				{"003-004", ErrorType.EmailIsTaken},
			};

		public static readonly Dictionary<string, ErrorType> ResetPasswordErrors =
			new Dictionary<string, ErrorType>()
			{
				{"003-007", ErrorType.PasswordResetingNotAllowedForProject},
				{"003-024", ErrorType.PasswordResetingNotAllowedForProject},
			};

		public static readonly Dictionary<string, ErrorType> TokenErrors =
			new Dictionary<string, ErrorType>()
			{
				{"422", ErrorType.TokenVerificationException},
			};
	}
}
