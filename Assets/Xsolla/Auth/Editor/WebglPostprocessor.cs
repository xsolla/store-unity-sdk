#if UNITY_WEBGL
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

			var sourcePath = GetSourceFilePath();
			Debug.Log($"Source path: {sourcePath}");

			if (!File.Exists(sourcePath))
				throw new FileNotFoundException($"Can't find {nameof(WebglPostprocessor)} script");

			var reportPath = report.summary.outputPath;
			var destPath = Path.Combine(reportPath, "xl-widget.html");
			Debug.Log("Destination path: " + destPath);

			if (File.Exists(destPath))
				File.Delete(destPath);

			File.Copy(sourcePath, destPath);
		}

		private static string GetSourceFilePath()
		{
			var guids = AssetDatabase.FindAssets($"t:Script {nameof(WebglPostprocessor)}");
			if (guids.Length == 0)
				throw new FileNotFoundException($"Can't find {nameof(WebglPostprocessor)} script");

			var path = AssetDatabase.GUIDToAssetPath(guids[0]);
			path = path.Replace("Assets", Application.dataPath);

			Debug.Log(path);

			path = Path.GetDirectoryName(path);
			if (path == null)
				throw new DirectoryNotFoundException("Can't find directory with android native file");
			
			return Path.Combine(path, "widget-proxy-page.html");
		}
	}
}

#endif