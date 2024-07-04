#if UNITY_ANDROID
using System.IO;

namespace Xsolla.DevTools
{
	public static class MainTemplateProcessor
	{
		public static void Process()
		{
			// var mainTemplatePath = ProvideMainTemplate();
			// if (mainTemplatePath != null)
			// 	PatchMainTemplate(mainTemplatePath);

			var sourceFilename = GetSourceFilename();
			if (sourceFilename == null)
				return;

			var sourcePath = Path.Combine(Utils.GetWorkDir(), "Templates", sourceFilename);
			var targetPath = Path.Combine(Utils.GetTargetDir(), "mainTemplate.gradle");
			Utils.CopyFileIfNotExists(sourcePath, targetPath);
		}

		private static string GetSourceFilename()
		{
#if UNITY_2022_3_OR_NEWER
			return "mainTemplate-2022.gradle";
#elif UNITY_2021_3_OR_NEWER
			return "mainTemplate-2021.gradle";
#elif UNITY_2020_3_OR_NEWER
			return "mainTemplate-2020.gradle";
#else
			return "mainTemplate-2019.gradle";
#endif
		}

		private static void PatchMainTemplate(string filePath)
		{
			var patchText = GetGradlePatchText();
			var originText = File.ReadAllText(filePath);
			if (originText.Contains(patchText))
				return;

			var finalText = originText + patchText;
			File.WriteAllText(filePath, finalText);
		}

		private static string GetGradlePatchText()
		{
			var workDir = Utils.GetWorkDir();
			var patchFilePath = Path.Combine(workDir, "Templates", "gradle-patch.txt");
			return File.ReadAllText(patchFilePath);
		}

		private static string ProvideMainTemplate()
		{
			var sourceFileName = GetSourceFilename();
			if (sourceFileName == null)
				return null;

			var workDir = Utils.GetWorkDir();
			var sourceFilePath = Path.Combine(workDir, "Templates", sourceFileName);

			var targetDir = Utils.GetTargetDir();
			var targetFilePath = Path.Combine(targetDir, "mainTemplate.gradle");
			Utils.CopyFileIfNotExists(sourceFilePath, targetFilePath);
			return targetFilePath;
		}
	}
}
#endif