#if UNITY_ANDROID
using System.IO;

namespace Xsolla.DevTools
{
	public static class BaseProjectTemplateProcessor
	{
		public static void Process()
		{
			var sourceFilename = GetSourceFilename();
			if (sourceFilename == null)
				return;

			var sourcePath = Path.Combine(Utils.GetWorkDir(), "Templates", sourceFilename);
			var targetPath = Path.Combine(Utils.GetTargetDir(), "baseProjectTemplate.gradle");
			Utils.CopyFileIfNotExists(sourcePath, targetPath);
		}

		private static string GetSourceFilename()
		{
#if UNITY_2022_3_OR_NEWER
			return null;
#else
			return "baseProjectTemplate-2019.gradle";
#endif
		}
	}
}
#endif