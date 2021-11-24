using System;
using UnityEngine;

namespace Xsolla.Demo
{
	public class UrlContainer : MonoBehaviour
	{
		[SerializeField] public string[] Urls = default(string[]);

		public string GetUrl(UrlType urlType)
		{
			var index = (int)urlType;
			return Urls[index];
		}

		private void Awake()
		{
			CheckConsistency();
		}

		private void CheckConsistency()
		{
			var typeNames = Enum.GetNames(typeof(UrlType));
			var numberOfElementsInEnum = typeNames.Length;
			var urlsArrayLength = Urls.Length;

			if (urlsArrayLength != numberOfElementsInEnum)
				Debug.LogError(string.Format("URLs length is {0} while there are {1} UrlTypes defined. Expect the IndexOutOfRangeException.", urlsArrayLength, numberOfElementsInEnum));

			for (int i = 0; i < urlsArrayLength; i++)
			{
				if (string.IsNullOrEmpty(Urls[i]))
					Debug.Log(string.Format("URL value for '{0}' is null or empty.", typeNames[i]));
			}
		}
	}
}
