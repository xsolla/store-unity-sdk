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
#if UNITY_ANDROID && UNITY_2019
			Debug.Log("Generate gradle templates");
			GenerateGradleTemplate(LauncherTemplateFileName);
			GenerateGradleTemplate(MainTemplateFileName);
#endif
		}

		public void OnPostprocessBuild(BuildReport report)
		{
#if UNITY_ANDROID && UNITY_2019
			Debug.Log("Delete gradle templates");
			DeleteGradleTemplate(LauncherTemplateFileName);
			DeleteGradleTemplate(MainTemplateFileName);
#endif
		}

		private void GenerateGradleTemplate(string fileName)
		{
			var sourcePath = GetSourcePath(fileName);
			var targetPath = GetTargetPath(fileName);
			if (!File.Exists(targetPath))
				File.Copy(sourcePath, targetPath);
		}

		private void DeleteGradleTemplate(string fileName)
		{
			var targetPath = GetTargetPath(fileName);
			if (File.Exists(targetPath))
				File.Delete(targetPath);
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