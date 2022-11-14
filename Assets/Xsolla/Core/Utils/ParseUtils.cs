using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Xsolla.Core
{
	public static class ParseUtils
	{
		[PublicAPI]
		public static T FromJson<T>(string json) where T : class
		{
			T result = default;

			try
			{
				result = JsonConvert.DeserializeObject<T>(json);
			}
			catch (Exception ex)
			{
				Debug.LogWarning($"Deserialization failed for {typeof(T)}");
				Debug.LogException(ex);
				result = null;
			}
	
			return result;
		}
		
		public static Error ParseError(string json)
		{
			if (json.Contains("statusCode") && json.Contains("errorCode") && json.Contains("errorMessage"))
				return FromJson<Error>(json);

			if (json.Contains("error") && json.Contains("code") && json.Contains("description"))
				return FromJson<LoginError>(json).ToError();

			return null;
		}

		public static bool TryParseError(string json, out Error error)
		{
			if (string.IsNullOrEmpty(json))
			{
				error = null;
				return false;
			}

			try
			{
				error = ParseError(json);
				return error != null;
			}
			catch (System.Exception ex)
			{
				error = new Error(errorType: ErrorType.InvalidData, errorMessage: ex.Message);
				return true;
			}
		}

		public static bool TryGetValueFromUrl(string url, ParseParameter parameter, out string value)
		{
			var parameterName = parameter.ToString();
			var regex = new Regex($"[&?]{parameterName}=[a-zA-Z0-9._+-]+");
			value = regex.Match(url)?.Value?.Replace($"{parameterName}=",string.Empty).Replace("&",string.Empty).Replace("?",string.Empty);

			switch (parameter)
			{
				case ParseParameter.error_code:
				case ParseParameter.error_description:
					if (value != null)
						value = value.Replace("+"," ");
					break;
				default:
					Debug.Log($"Trying to find {parameterName} in URL:{url}");
					break;
			}

			return !string.IsNullOrEmpty(value);
		}
	}	
}
