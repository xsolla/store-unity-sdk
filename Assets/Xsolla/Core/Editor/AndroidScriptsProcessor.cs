#if UNITY_ANDROID
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Xsolla.Core.Editor
{
	public class AndroidScriptsProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
	{
		public int callbackOrder => 2000;

		public void OnPreprocessBuild(BuildReport report)
		{
			if (report.summary.platform != BuildTarget.Android)
				return;

			XDebug.Log("Xsolla SDK is now processing android activities", true);
			SetupActivities(Application.identifier);
		}

		public void OnPostprocessBuild(BuildReport report)
		{
			if (report.summary.platform == BuildTarget.Android)
				SetupActivities("com.xsolla.sdk.unity.Example");
		}

		private void SetupActivities(string appId)
		{
			SetupActivity("PaymentsProxyActivity", true, appId, "androidProxies");
			SetupActivity("SocialAuthProxyActivity", true, appId, "androidProxies");
			SetupActivity("XsollaWidgetAuthProxyActivity", true, appId, "androidProxies");
			SetupActivity("WXEntryActivity", !string.IsNullOrEmpty(XsollaSettings.WeChatAppId), appId, "wxapi");
		}

		private static void SetupActivity(string activityName, bool enableCondition, string appId, string packageSuffix)
		{
			var assetPath = GetAssetPath(activityName);
			var activityAsset = AssetImporter.GetAtPath(assetPath) as PluginImporter;
			if (activityAsset != null)
			{
				activityAsset.SetCompatibleWithPlatform(BuildTarget.Android, enableCondition);
				activityAsset.SaveAndReimport();
			}

			var filePath = GetFilePath(activityName);
			var scriptContent = File.ReadAllText(filePath);
			var packageLineText = $"package {appId}.{packageSuffix};";
			scriptContent = Regex.Replace(scriptContent, "package.+;", packageLineText);

			File.WriteAllText(filePath, scriptContent);
		}

		private static string GetAssetPath(string fileName)
		{
			var path = GetFilePath(fileName);
			path = path.Substring(Application.dataPath.Length);
			return "Assets" + path;
		}

		private static string GetFilePath(string fileName)
		{
			var guids = AssetDatabase.FindAssets($"t:Script {nameof(AndroidScriptsProcessor)}");
			if (guids.Length == 0)
				throw new FileNotFoundException($"Can't find {nameof(AndroidScriptsProcessor)} script");

			var path = AssetDatabase.GUIDToAssetPath(guids[0]);
			path = path.Replace("Assets", Application.dataPath);

			path = Path.GetDirectoryName(path);
			if (path == null)
				throw new DirectoryNotFoundException("Can't find directory with android native file");

			path = Directory.GetParent(path)?.FullName;
			if (path == null)
				throw new DirectoryNotFoundException("Can't find directory with android native file");

			return Path.Combine(path, "Plugins", "Android", $"{fileName}.java");
		}
	}
}
#endif