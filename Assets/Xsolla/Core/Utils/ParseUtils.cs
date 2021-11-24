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
				Debug.LogWarning(string.Format("Deserialization failed for {0}", typeof(T)));
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
				if (loginError != null)
					storeError = loginError.ToStoreError();
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
			Debug.Log(string.Format("Trying to find {0} in URL:{1}", parameterName, url));

			var regex = new Regex(string.Format("[&?]{0}=[a-zA-Z0-9._-]+", parameterName));
			var match = regex.Match(url);
			value = null;
			if (match != null && match.Value != null)
				value = match.Value.Replace(string.Format("{0}=", parameterName), string.Empty).Replace("&", string.Empty).Replace("?", string.Empty);
			return !string.IsNullOrEmpty(value);
		}
	}	
}
