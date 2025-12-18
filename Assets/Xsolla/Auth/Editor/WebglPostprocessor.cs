#if UNITY_WEBGL
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Xsolla.Core.Editor
{
	public class WebglPostprocessor : IPostprocessBuildWithReport
	{
		public int callbackOrder => 1000;

		public void OnPostprocessBuild(BuildReport report)
		{
			if (report.summary.platform != BuildTarget.WebGL)
				return;

			XDebug.Log("SDK is now processing build", true);

			var sourceDir = GetSourceDir();
			var outputDir = report.summary.outputPath;
			CopyFile("xl-widget.html", sourceDir, outputDir);
			CopyFile("xl-social.html", sourceDir, outputDir);
		}

		private static void CopyFile(string fileName, string sourceDir, string outputDir)
		{
			var sourcePath = Path.Combine(sourceDir, fileName);
			Debug.Log($"Source path: {sourcePath}");

			if (!File.Exists(sourcePath))
				throw new Exception($"Can't find source file: {sourcePath}");

			var outputPath = Path.Combine(outputDir, fileName);
			Debug.Log("Output path: " + outputPath);

			File.Copy(sourcePath, outputPath, true);
		}

		private static string GetSourceDir()
		{
			var scriptName = nameof(WebglPostprocessor);

			var guids = AssetDatabase.FindAssets($"t:Script {scriptName}");
			if (guids.Length == 0)
				throw new Exception($"Can't find {scriptName} script");

			var path = AssetDatabase.GUIDToAssetPath(guids[0]);
			path = path.Replace("Assets", Application.dataPath);

			path = Path.GetDirectoryName(path);
			if (path == null)
				throw new Exception($"Can't find directory of {scriptName} script");

			return path;
		}
	}
}

#endif