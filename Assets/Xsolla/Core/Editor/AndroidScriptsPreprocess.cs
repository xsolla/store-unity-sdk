using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Xsolla.Core.Editor
{
	public class AndroidScriptsPreprocess : IPreprocessBuildWithReport
	{
		public int callbackOrder => 3000;

		public void OnPreprocessBuild(BuildReport report)
		{
			if (report.summary.platform != BuildTarget.Android)
				return;

			SetupActivity("PaymentsProxyActivity", true, "androidProxies");
			SetupActivity("SocialAuthProxyActivity", true, "androidProxies");
			SetupActivity("XsollaWidgetAuthProxyActivity", true, "androidProxies");
			SetupActivity("WXEntryActivity", !string.IsNullOrEmpty(XsollaSettings.WeChatAppId), "wxapi");
		}

		private static void SetupActivity(string activityName, bool enableCondition, string packageSuffix)
		{
			var activityScriptPath = GetScriptFilePath($"{activityName}.java");
			if (!File.Exists(activityScriptPath))
			{
				XDebug.LogError($"Xsolla {activityName} activity script is missing!");
				return;
			}

			var assetPath = "Assets" + activityScriptPath.Substring(Application.dataPath.Length);
			var activityAsset = AssetImporter.GetAtPath(assetPath) as PluginImporter;
			if (activityAsset != null)
			{
				activityAsset.SetCompatibleWithPlatform(BuildTarget.Android, enableCondition);
				activityAsset.SaveAndReimport();
			}

			var scriptContent = File.ReadAllText(activityScriptPath);
			var packageLineText = $"package {Application.identifier}.{packageSuffix};";
			scriptContent = Regex.Replace(scriptContent, "package.+;", packageLineText);

			File.WriteAllText(activityScriptPath, scriptContent);
		}

		private static string GetScriptFilePath(string fileName)
		{
			var path = GetScriptsDirectory(Application.dataPath);
			return path != null
				? Path.Combine(path, fileName)
				: null;
		}

		private static string GetScriptsDirectory(string path)
		{
			foreach (var dir in Directory.GetDirectories(path))
			{
				if (dir.Contains("AndroidNativeScripts"))
					return dir;

				var recursiveSearchResult = GetScriptsDirectory(dir);
				if (recursiveSearchResult != null)
					return recursiveSearchResult;
			}

			return null;
		}
	}
}