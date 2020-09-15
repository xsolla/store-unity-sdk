using System;
using System.Collections.Generic;

namespace Xsolla.Core
{
	[Serializable]
	public partial class Error
	{
		public string statusCode;
		public string errorCode;
		public string errorMessage;
		public ErrorType ErrorType { get; set; }

		public Error(ErrorType errorType = ErrorType.UnknownError, string statusCode = "", string errorMessage = "", string errorCode = "")
		{
			this.statusCode = statusCode;
			this.errorMessage = errorMessage;
			this.errorCode = errorCode;
			ErrorType = errorType;
		}

		public static Error NetworkError
		{
			get { return new Error(ErrorType.NetworkError); }
		}
		
		public static Error UnknownError
		{
			get { return new Error(ErrorType.UnknownError); }
		}

		public static readonly Dictionary<string, ErrorType> GeneralErrors =
			new Dictionary<string, ErrorType>()
			{
				{"403", ErrorType.InvalidToken},
				{"405", ErrorType.MethodIsNotAllowed},

				{"0", ErrorType.InvalidProjectSettings},
				{"003-001", ErrorType.InvalidLoginOrPassword},
				{"003-061", ErrorType.InvalidProjectSettings},
				{"010-011", ErrorType.MultipleLoginUrlsException},
				{"010-012", ErrorType.SubmittedLoginUrlNotFoundException}
			};

		public bool IsValid()
		{
			return (statusCode != null) || (errorCode != null) || (errorMessage != null);
		}
		
		public override string ToString()
		{
			var message = string.Empty;
			if (ErrorType != ErrorType.UnknownError)
				message += $"Type: {ErrorType}. ";
			if (!string.IsNullOrEmpty(statusCode))
				message += $"Status code: {statusCode}. ";
			if (!string.IsNullOrEmpty(errorCode))
				message += $"Error code: {errorCode}. ";
			if (!string.IsNullOrEmpty(errorMessage))
				message += $"Message: {errorMessage}. ";
			return message;
		}
	}
}
