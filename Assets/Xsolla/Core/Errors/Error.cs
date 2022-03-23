using System;
using System.Text;

namespace Xsolla.Core
{
	[Serializable]
	public partial class Error
	{
		public string statusCode;
		public string errorCode;
		public string errorMessage;

		public ErrorType ErrorType { get; set; } = ErrorType.UnknownError;

		public Error(ErrorType errorType = ErrorType.UnknownError, string statusCode = "", string errorCode = "", string errorMessage = "")
		{
			this.statusCode = statusCode;
			this.errorCode = errorCode;
			this.errorMessage = errorMessage;
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

		public bool IsValid()
		{
			return	(statusCode != null) ||
					(errorCode != null) ||
					(errorMessage != null) ||
					(ErrorType != ErrorType.Undefined);
		}
		
		public override string ToString()
		{
			var builder = new StringBuilder();

			if (ErrorType != ErrorType.UnknownError)
				builder.Append($"Type: {ErrorType}. ");
			if (!string.IsNullOrEmpty(statusCode))
				builder.Append($"Status code: {statusCode}. ");
			if (!string.IsNullOrEmpty(errorCode))
				builder.Append($"Error code: {errorCode}. ");
			if (!string.IsNullOrEmpty(errorMessage))
				builder.Append($"{errorMessage}. ");

			return builder.ToString();
		}
	}
}
