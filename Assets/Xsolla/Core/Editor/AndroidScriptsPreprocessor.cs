using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Xsolla.Core
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
			SetupWechatActivity();
#endif
		}

		void SetupWechatActivity()
		{
			var wechatActivityScriptPath = Path.Combine(FindAndroidScripts(Application.dataPath).Replace("\\", "/"), "WXEntryActivity.java");

			if (!File.Exists(wechatActivityScriptPath))
			{
				Debug.LogError("WeChat Android activity script is missing.");
				return;
			}

			var scriptContent = File.ReadAllText(wechatActivityScriptPath);

			var androidPackageName = Application.identifier;
			var editedScriptContent = Regex.Replace(scriptContent, "package.+;", string.Format("package {0}.wxapi;", androidPackageName));

			File.WriteAllText(wechatActivityScriptPath, editedScriptContent);
		}

		static string FindAndroidScripts(string path)
		{
			foreach (var dir in Directory.GetDirectories(path))
			{
				if (dir.Contains("AndroidNativeScripts"))
				{
					return dir;
				}

				var rec = FindAndroidScripts(dir);
				if (rec != null)
				{
					return rec;
				}
			}

			return null;
		}
	}
}