using System;
using System.Collections.Generic;
using Xsolla.Core;

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
		
		public static ErrorDescription NetworkError
		{
			get { return new ErrorDescription("-", "Network error", Error.NetworkError); }
		}
		
		public static ErrorDescription UnknownError
		{
			get { return new ErrorDescription("-", "Unknown error", Error.UnknownError); }
		}
		
		public static readonly Dictionary<string, Error> GeneralErrors =
			new Dictionary<string, Error>()
			{
				{"0", Error.InvalidProjectSettings},
				{"003-001", Error.InvalidLoginOrPassword},
				{"003-061", Error.InvalidProjectSettings},
				{"010-011", Error.MultipleLoginUrlsException},
				{"010-012", Error.SubmittedLoginUrlNotFoundException},
			};
		
		public static readonly Dictionary<string, Error> LoginErrors =
			new Dictionary<string, Error>()
			{
				{"003-007", Error.UserIsNotActivated},
				{"010-007", Error.CaptchaRequiredException},
			};
		
		public static readonly Dictionary<string, Error> RegistrationErrors =
			new Dictionary<string, Error>()
			{
				{"010-003", Error.RegistrationNotAllowedException},
				{"003-003", Error.UsernameIsTaken},
				{"003-004", Error.EmailIsTaken},
			};

		public static readonly Dictionary<string, Error> ResetPasswordErrors =
			new Dictionary<string, Error>()
			{
				{"003-007", Error.PasswordResetingNotAllowedForProject},
				{"003-024", Error.PasswordResetingNotAllowedForProject},
			};
		
		public static readonly Dictionary<string, Error> TokenErrors =
			new Dictionary<string, Error>()
			{
				{"422", Error.TokenVerificationException},
			};
		
		public override string ToString()
		{
			return string.Format("Code: {0}. Description: {1}", code, error);
		}
	}
}