using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Xsolla.Core.Editor
{
	public class AndroidManifestPreprocess : IPreprocessBuildWithReport
	{
		public int callbackOrder => 2000;

		public void OnPreprocessBuild(BuildReport report)
		{
			if (report.summary.platform != BuildTarget.Android)
				return;

			XDebug.Log("Xsolla SDK is now processing AndroidManifest", true);
			SetupWeChat();
			SetupProxyActivity("PaymentsProxyActivity");
			SetupProxyActivity("SocialAuthProxyActivity");
			SetupProxyActivity("XsollaWidgetAuthProxyActivity");
		}

		private static void SetupWeChat()
		{
			var androidPackageName = Application.identifier;
			var wechatActivityName = $"{androidPackageName}.wxapi.WXEntryActivity";
			var wechatActivity = new ActivityNode(wechatActivityName);
			wechatActivity.AddAttribute(AndroidManifestConstants.ExportedAttribute, "true");

			var manifest = LoadManifestWrapper();
			var manifestChanged = false;

			// cleanup manifest in case WeChat activity was added previously
			if (manifest.ContainsNode(new FindByTag(AndroidManifestConstants.ApplicationTag), new FindByName(wechatActivity)))
			{
				manifest.RemoveNode(new FindByTag(AndroidManifestConstants.ApplicationTag), new FindByName(wechatActivity));
				manifestChanged = true;
			}

			if (!string.IsNullOrEmpty(XsollaSettings.WeChatAppId))
			{
				manifest.AddNode(wechatActivity, new FindByTag(AndroidManifestConstants.ApplicationTag));
				manifestChanged = true;
			}

			if (manifestChanged)
				manifest.SaveManifest();
		}

		private static void SetupProxyActivity(string activityName)
		{
			var fullActivityName = $"{Application.identifier}.androidProxies.{activityName}";
			var activityNode = new ActivityNode(fullActivityName);
			activityNode.AddAttribute(AndroidManifestConstants.ExportedAttribute, "true");
			activityNode.AddAttribute(AndroidManifestConstants.ConfigChanges, "orientation|screenSize|keyboardHidden");

			var manifest = LoadManifestWrapper();

			// cleanup manifest in case activity node was added previously
			if (manifest.ContainsNode(new FindByTag(AndroidManifestConstants.ApplicationTag), new FindByName(activityNode)))
				manifest.RemoveNode(new FindByTag(AndroidManifestConstants.ApplicationTag), new FindByName(activityNode));

			manifest.AddNode(activityNode, new FindByTag(AndroidManifestConstants.ApplicationTag));
			manifest.SaveManifest();
		}

		private static AndroidManifestWrapper LoadManifestWrapper()
		{
			var manifestPath = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");
			if (!File.Exists(manifestPath))
			{
				var manifestDirectory = Path.GetDirectoryName(manifestPath);
				if (manifestDirectory != null && !Directory.Exists(manifestDirectory))
					Directory.CreateDirectory(manifestDirectory);

				var templatePath = GetTemplateFilePath("AndroidManifest.xml");
				File.Copy(templatePath, manifestPath);
			}

			return new AndroidManifestWrapper(manifestPath);
		}

		private static string GetTemplateFilePath(string fileName)
		{
			var path = GetTemplatesDirectory(Application.dataPath);
			return path != null
				? Path.Combine(path, fileName)
				: null;
		}

		private static string GetTemplatesDirectory(string path)
		{
			foreach (var dir in Directory.GetDirectories(path))
			{
				if (dir.Contains("AndroidTemplateFiles"))
					return dir;

				var recursiveSearchResult = GetTemplatesDirectory(dir);
				if (recursiveSearchResult != null)
					return recursiveSearchResult;
			}

			return null;
		}
	}
}