using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Xsolla
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
		
		public static XsollaError ParseError(string json)
		{
			return FromJson<XsollaError>(json);
		}

		public static XsollaStoreItems ParseStoreItems(string json)
		{
			return FromJson<XsollaStoreItems>(json);
		}
		
		public static XsollaToken ParseToken(string json)
		{
			return FromJson<XsollaToken>(json);
		}
	}	
}
