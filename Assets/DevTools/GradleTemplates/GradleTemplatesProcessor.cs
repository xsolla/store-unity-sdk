#if UNITY_ANDROID && UNITY_2019
using System;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Xsolla
{
	public class GradleTemplatesProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
	{
		public int callbackOrder => -1000;

		private const string LauncherTemplateFileName = "launcherTemplate.gradle";

		private const string MainTemplateFileName = "mainTemplate.gradle";

		public void OnPreprocessBuild(BuildReport report)
		{
			Debug.Log("Generate gradle templates");
			GenerateGradleTemplate(LauncherTemplateFileName);
			GenerateGradleTemplate(MainTemplateFileName);
		}

		public void OnPostprocessBuild(BuildReport report)
		{
			Debug.Log("Delete gradle templates");
			DeleteGradleTemplate(LauncherTemplateFileName);
			DeleteGradleTemplate(MainTemplateFileName);
		}

		private void GenerateGradleTemplate(string fileName)
		{
			var sourcePath = GetSourcePath(fileName);
			var targetPath = GetTargetPath(fileName);

			var targetDir = Path.GetDirectoryName(targetPath);
			if (targetDir != null && !Directory.Exists(targetDir))
				Directory.CreateDirectory(targetDir);

			if (!File.Exists(targetPath))
				File.Copy(sourcePath, targetPath);
		}

		private void DeleteGradleTemplate(string fileName)
		{
			var targetPath = GetTargetPath(fileName);
			if (File.Exists(targetPath))
				File.Delete(targetPath);

			var metaFilePath = $"{targetPath}.meta";
			if (File.Exists(metaFilePath))
				File.Delete(metaFilePath);
		}

		private string GetSourcePath(string fileName)
		{
			return Path.Combine(Application.dataPath, "DevTools/GradleTemplates/", fileName);
		}

		private string GetTargetPath(string fileName)
		{
			return Path.Combine(Application.dataPath, "Plugins/Android/", fileName);
		}
	}
}
#endif