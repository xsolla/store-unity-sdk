using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla
{
	[Serializable]
	public class XsollaError
	{
		public string statusCode;
		public string errorCode;
		public string errorMessage;

		public ErrorType type;
		
		static readonly Dictionary<string, ErrorType> errors =
			new Dictionary<string, ErrorType>()
			{
				{"403", ErrorType.InvalidToken},
				{"405", ErrorType.MethodIsNotAllowed},
				{"422", ErrorType.InvalidData},
			};

		XsollaError()
		{
			Debug.Log("Status" + statusCode);
			if (!string.IsNullOrEmpty(statusCode))
			{
				Debug.Log("Status" + statusCode);
				
				if (errors.ContainsKey(statusCode))
				{
					type = errors[statusCode];
				}
				else
				{
					type = ErrorType.UnknownError;
				}
			}
		}

		public XsollaError(ErrorType errorType, string message)
		{
			type = errorType;
			errorMessage = message;
		}
		
		public override string ToString()
		{
			return string.Format("Error - Type: {0}. Status: {1}. Error code: {2}. Message: {3}.", type, statusCode, errorCode, errorMessage);
		}
	}
}
