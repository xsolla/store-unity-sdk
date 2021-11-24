using System;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Xsolla.Core
{
	public class AndroidManifestPreprocessor : IPreprocessBuildWithReport
	{
		const string MainManifestPath = "Plugins/Android/AndroidManifest.xml";
		const string XsollaManifestLabel = "xsolla";

		public int callbackOrder
		{
			get { return 2000; }
		}

		public void OnPreprocessBuild(BuildReport report)
		{
#if UNITY_ANDROID
			Debug.Log("Xsolla SDK is now preprocessing your AndroidManifest.xml");
			SetupDeepLinking();
			SetupWeChat();
#endif
		}

		void SetupDeepLinking()
		{
			var manifestPath = Path.Combine(Application.dataPath, MainManifestPath);
			var manifestExists = File.Exists(manifestPath);

			if (!manifestExists)
			{
				if (!XsollaSettings.UseDeepLinking)
				{
					return;
				}

				RestoreAndroidManifest(manifestPath);
			}

			var manifest = new AndroidManifestWrapper(manifestPath);

			if (XsollaSettings.UseDeepLinking && string.IsNullOrEmpty(XsollaSettings.DeepLinkRedirectUrl))
			{
				Debug.LogError("Redirect URL for Android deep linking is empty. Please check plugin settings.");
				return;
			}

			var redirectUrl = !string.IsNullOrEmpty(XsollaSettings.DeepLinkRedirectUrl) ? new Uri(XsollaSettings.DeepLinkRedirectUrl) : null;

			var scheme	= default(string);
			var host	= default(string);
			var pathAndQuery = default(string);

			if (redirectUrl != null)
			{
				scheme = redirectUrl.Scheme;
				host = redirectUrl.Host;
				pathAndQuery = redirectUrl.PathAndQuery;
			}

			var data = new DataNode(scheme, host, pathAndQuery);
			var action = new ActionNode("android.intent.action.VIEW");
			var categoryDefault = new CategoryNode("android.intent.category.DEFAULT");
			var categoryBrowsable = new CategoryNode("android.intent.category.BROWSABLE");

			var intentFilter = new IntentFilterNode(AndroidManifestConstants.ActivityTag);
			intentFilter.AddAttribute(AndroidManifestConstants.LabelAttribute, XsollaManifestLabel);
			intentFilter.AddChildNode(data);
			intentFilter.AddChildNode(action);
			intentFilter.AddChildNode(categoryDefault);
			intentFilter.AddChildNode(categoryBrowsable);

			var manifestChanged = false;

			// action node used to identify main game activity
			var mainActivityAction = new ActionNode("android.intent.action.MAIN");

			// cleanup manifest in case deep linking settings were added previously
			if (manifest.ContainsNode(new FindByChildName(AndroidManifestConstants.ActivityTag, mainActivityAction), new FindByLabel(intentFilter)))
			{
				manifest.RemoveNode(new FindByChildName(AndroidManifestConstants.ActivityTag, mainActivityAction), new FindByLabel(intentFilter));
				manifestChanged = true;
			}

			if (XsollaSettings.UseDeepLinking)
			{
				manifest.AddNode(intentFilter, new FindByChildName(AndroidManifestConstants.ActivityTag, mainActivityAction));
				manifestChanged = true;
			}

			if (manifestChanged)
			{
				manifest.SaveManifest();
			}
		}

		void SetupWeChat()
		{
			var manifestPath = Path.Combine(Application.dataPath, MainManifestPath);
			var manifestExists = File.Exists(manifestPath);

			if (!manifestExists)
			{
				if (string.IsNullOrEmpty(XsollaSettings.WeChatAppId))
				{
					return;
				}

				RestoreAndroidManifest(manifestPath);
			}

			var manifest = new AndroidManifestWrapper(manifestPath);

			var androidPackageName = Application.identifier;
			var wechatActivityName = string.Format("{0}.wxapi.WXEntryActivity", androidPackageName);

			var wechatActivity = new ActivityNode(wechatActivityName);
			wechatActivity.AddAttribute(AndroidManifestConstants.ExportedAttribute, "true");

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
			{
				manifest.SaveManifest();
			}
		}

		static void RestoreAndroidManifest(string manifestPath)
		{
			var backupManifestPath = Path.Combine(FindAndroidManifestBackup(Application.dataPath).Replace("\\", "/"), "AndroidManifest.xml");

			var manifestDirectoryPath = Path.GetDirectoryName(manifestPath);
			if (!Directory.Exists(manifestDirectoryPath))
			{
				Directory.CreateDirectory(manifestDirectoryPath);
			}

			File.Copy(backupManifestPath, manifestPath);
		}

		static string FindAndroidManifestBackup(string path)
		{
			foreach (var dir in Directory.GetDirectories(path))
			{
				if (dir.Contains("AndroidManifestBackup"))
				{
					return dir;
				}

				var rec = FindAndroidManifestBackup(dir);
				if (rec != null)
				{
					return rec;
				}
			}

			return null;
		}
	}
}
