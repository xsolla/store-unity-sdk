#if UNITY_ANDROID
using System.IO;

namespace Xsolla.DevTools
{
	public static class GradleTemplateProcessor
	{
		public static void Process()
		{
			var sourceFilename = GetSourceFilename();
			if (sourceFilename == null)
				return;

			var sourcePath = Path.Combine(Utils.GetWorkDir(), "Templates", sourceFilename);
			var targetPath = Path.Combine(Utils.GetTargetDir(), "gradleTemplate.properties");
			Utils.CopyFileIfNotExists(sourcePath, targetPath);
		}

		private static string GetSourceFilename()
		{
#if UNITY_6000_0_OR_NEWER
			return "gradleTemplate-6000.properties";
#elif UNITY_2022_3_OR_NEWER
			return "gradleTemplate-2022.properties";
#elif UNITY_2021_3_OR_NEWER
			return "gradleTemplate-2021.properties";
#elif UNITY_2020_3_OR_NEWER
			return "gradleTemplate-2020.properties";
#else
			return "gradleTemplate-2019.properties";
#endif
		}
	}
}
#endif