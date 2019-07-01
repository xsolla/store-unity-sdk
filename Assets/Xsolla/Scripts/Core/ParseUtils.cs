using System;
using JetBrains.Annotations;
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
				return (T) JsonUtility.FromJson(json, typeof(T));
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
	
			return null;
		}
		
		public static Error ParseError(string json)
		{
			return FromJson<Error>(json);
		}
	}	
}
