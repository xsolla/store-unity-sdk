#if UNITY_ANDROID
using System.IO;

namespace Xsolla.DevTools
{
	public static class LauncherTemplateProcessor
	{
		public static void Process()
		{
			// var launcherTemplatePath = ProvideLauncherTemplate();
			// if (launcherTemplatePath != null)
			// 	PatchLauncherTemplate(launcherTemplatePath);
			
			var sourceFilename = GetLauncherSourceFileName();
			if (sourceFilename == null)
				return;
			
			var sourcePath = Path.Combine(Utils.GetWorkDir(), "Templates", sourceFilename);
			var targetPath = Path.Combine(Utils.GetTargetDir(), "launcherTemplate.gradle");
			Utils.CopyFileIfNotExists(sourcePath, targetPath);
		}

		private static string GetLauncherSourceFileName()
		{
#if UNITY_2022_3_OR_NEWER
			return "launcherTemplate-2022.gradle";
#elif UNITY_2021_3_OR_NEWER
			return "launcherTemplate-2021.gradle";
#elif UNITY_2020_3_OR_NEWER
			return "launcherTemplate-2020.gradle";
#else
			return "launcherTemplate-2019.gradle";
#endif
		}

		private static string GetGradlePatchText()
		{
			var workDir = Utils.GetWorkDir();
			var patchFilePath = Path.Combine(workDir, "Templates", "gradle-patch.txt");
			return File.ReadAllText(patchFilePath);
		}

		private static string ProvideLauncherTemplate()
		{
			var sourceFileName = GetLauncherSourceFileName();
			if (sourceFileName == null)
				return null;

			var workDir = Utils.GetWorkDir();
			var sourceFilePath = Path.Combine(workDir, "Templates", sourceFileName);

			var targetDir = Utils.GetTargetDir();
			var targetFilePath = Path.Combine(targetDir, "launcherTemplate.gradle");
			Utils.CopyFileIfNotExists(sourceFilePath, targetFilePath);
			return targetFilePath;
		}

		private static void PatchLauncherTemplate(string filePath)
		{
			var patchText = GetGradlePatchText();
			var originText = File.ReadAllText(filePath);
			if (originText.Contains(patchText))
				return;

			var finalText = originText + patchText;
			File.WriteAllText(filePath, finalText);
		}
	}
}
#endif