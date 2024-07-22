#if UNITY_ANDROID
using System.IO;
using UnityEditor.Android;
using UnityEngine;

namespace Xsolla.DevTools
{
	public static class GradlePathProcessor
	{
		public static void Process()
		{
			var gradleVersion = GetGradleVersion();
			var path = Application.dataPath.Replace("Assets", "gradle");
			path = Path.Combine(path, $"gradle-{gradleVersion}");
			AndroidExternalToolsSettings.gradlePath = path;
		}

		private static string GetGradleVersion()
		{
#if UNITY_6000_0_OR_NEWER
			return "8.4";
#else
			return "7.2";
#endif
		}
	}
}
#endif