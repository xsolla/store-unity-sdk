using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Xsolla.Core.Editor
{
	public class AndroidScriptsPreprocessor : IPreprocessBuildWithReport
	{
		public int callbackOrder
		{
			get { return 3000; }
		}

		public void OnPreprocessBuild(BuildReport report)
		{
#if UNITY_ANDROID
			Debug.Log("Xsolla SDK is now preprocessing native Android scripts.");
			SetupActivity("WXEntryActivity",      enableCondition:!string.IsNullOrEmpty(XsollaSettings.WeChatAppId), packageSuffix:"wxapi");
			SetupActivity("AndroidPaymentsProxy", enableCondition:true, packageSuffix:"androidProxies");
			SetupActivity("AndroidAuthProxy",     enableCondition:true, packageSuffix:"androidProxies");
#endif
		}

		private void SetupActivity(string activityName, bool enableCondition, string packageSuffix)
		{
			var activityScriptPath = Path.Combine(FindAndroidScripts(Application.dataPath).Replace("\\", "/"), $"{activityName}.java");

			if (!File.Exists(activityScriptPath))
			{
				Debug.LogError($"{activityName} activity script is missing.");
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

			var androidPackageName = Application.identifier;
			var editedScriptContent = Regex.Replace(scriptContent, "package.+;", $"package {androidPackageName}.{packageSuffix};");

			File.WriteAllText(activityScriptPath, editedScriptContent);
		}

		private static string FindAndroidScripts(string path)
		{
			foreach (var dir in Directory.GetDirectories(path))
			{
				if (dir.Contains("AndroidNativeScripts"))
				{
					return dir;
				}

				var recursiveSearchResult = FindAndroidScripts(dir);
				if (recursiveSearchResult != null)
				{
					return recursiveSearchResult;
				}
			}

			return null;
		}
	}
}