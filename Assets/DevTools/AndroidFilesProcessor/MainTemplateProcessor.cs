#if UNITY_ANDROID
using System.IO;

namespace Xsolla.DevTools
{
	public static class MainTemplateProcessor
	{
		public static void Process()
		{
			var sourceFilename = GetSourceFilename();
			if (sourceFilename == null)
				return;

			var sourcePath = Path.Combine(Utils.GetWorkDir(), "Templates", sourceFilename);
			var targetPath = Path.Combine(Utils.GetTargetDir(), "mainTemplate.gradle");
			Utils.CopyFileIfNotExists(sourcePath, targetPath);
		}

		private static string GetSourceFilename()
		{
#if UNITY_6000_0_OR_NEWER
			return "mainTemplate-6000.gradle";
#elif UNITY_2022_3_OR_NEWER
			return "mainTemplate-2022.gradle";
#elif UNITY_2021_3_OR_NEWER
			return "mainTemplate-2021.gradle";
#elif UNITY_2020_3_OR_NEWER
			return "mainTemplate-2020.gradle";
#else
			return "mainTemplate-2019.gradle";
#endif
		}
	}
}
#endif