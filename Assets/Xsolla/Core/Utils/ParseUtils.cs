using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;


namespace Xsolla.Core
{
	public static class ParseUtils
	{
		[PublicAPI]
		public static T FromJson<T>(string json) where T : class
		{
			try
			{
				return JsonConvert.DeserializeObject<T>(json);
			}
			catch (Exception e)
			{
				Debug.LogWarning($"Deserialization failed for {typeof(T)}");
				Debug.LogException(e);
			}
	
			return null;
		}
		
		public static Error ParseError(string json)
		{
			try
			{
				if (JsonConvert.DeserializeObject(json) is JArray)
				{
					// if json is a simple array return null to avoid raising exception while trying to parse it as an error
					return null;
				}
			}
			catch (Exception ex)
			{
				//if this is not a json at all
				Debug.LogError(ex.Message);
				return null;
			}
			
			Error storeError = FromJson<Error>(json);
			if((storeError == null) || (!storeError.IsValid())) {
				Error.Login loginError = FromJson<Error.Login>(json);
				storeError = loginError?.ToStoreError();
			}
			return storeError;
		}
		
		public static string ParseToken(string token)
		{
			try
			{
				var regex = new Regex(@"token=\S*[&#]");
				var match = regex.Match(token).Value.Replace("token=", string.Empty);
				return match.Remove(match.Length - 1);
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}

		public static bool TryGetValueFromUrl(string url, ParseParameter parameter, out string value)
		{
			var parameterName = parameter.ToString();
			var regex = new Regex($"[&?]{parameterName}=[a-zA-Z0-9._+-]+");
			value = regex.Match(url)?.Value?.Replace($"{parameterName}=", string.Empty).Replace("&",string.Empty).Replace("?",string.Empty);

			switch (parameter)
			{
				case ParseParameter.error_code:
				case ParseParameter.error_description:
					if (value != null)
						value = value.Replace("+", " ");
					break;
				default:
					Debug.Log($"Trying to find {parameterName} in URL:{url}");
					break;
			}

			return !string.IsNullOrEmpty(value);
		}
	}	
}
