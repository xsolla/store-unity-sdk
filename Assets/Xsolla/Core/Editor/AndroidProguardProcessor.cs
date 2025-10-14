#if UNITY_ANDROID
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Xsolla.Core.Editor
{
	public class AndroidProguardProcessor : IPreprocessBuildWithReport
	{
		public int callbackOrder => 1000;

		public void OnPreprocessBuild(BuildReport report)
		{
			if (report.summary.platform != BuildTarget.Android)
				return;

			XDebug.Log("Xsolla SDK is now processing proguard settings", true);
			EnsureProguardRules();
		}

		private static void EnsureProguardRules()
		{
			var templateFilePath = GetTemplateFilePath();
			if (!File.Exists(templateFilePath))
			{
				Debug.LogError("Proguard template file not found: " + templateFilePath);
				return;
			}

			var targetFilePath = GetTargetFilePath();
			if (!File.Exists(targetFilePath))
			{
				var directoryName = Path.GetDirectoryName(targetFilePath);
				if (directoryName == null)
				{
					Debug.LogError("Can't get directory name for proguard file: " + targetFilePath);
					return;
				}

				if (!Directory.Exists(directoryName))
					Directory.CreateDirectory(directoryName);

				File.WriteAllText(targetFilePath, "");
			}

			var targetContent = File.ReadAllText(targetFilePath);
			var sourceLines = File.ReadAllLines(templateFilePath);

			using var writer = new StreamWriter(targetFilePath, append: true);
			foreach (var ruleLine in sourceLines)
			{
				var trimmedRuleLine = ruleLine.Trim();
				if (string.IsNullOrEmpty(trimmedRuleLine))
					continue;

				if (!targetContent.Contains(trimmedRuleLine))
					writer.WriteLine(trimmedRuleLine);
			}
		}

		private static string GetTargetFilePath()
		{
			return Path.Combine(Application.dataPath, "Plugins", "Android", "proguard-user.txt");
		}

		private static string GetTemplateFilePath()
		{
			var guids = AssetDatabase.FindAssets($"t:Script {nameof(AndroidProguardProcessor)}");
			if (guids.Length == 0)
				throw new FileNotFoundException($"Can't find {nameof(AndroidProguardProcessor)} script");

			var path = AssetDatabase.GUIDToAssetPath(guids[0]);
			path = path.Replace("Assets", Application.dataPath);

			path = Path.GetDirectoryName(path);
			if (path == null)
				throw new DirectoryNotFoundException("Can't find directory with android file templates");

			return Path.Combine(path, "AndroidTemplateFiles", "proguard-user.txt");
		}
	}
}
#endif