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

				var backupManifestPath = Path.Combine(FindAndroidManifestBackup(Application.dataPath).Replace("\\", "/"), "AndroidManifest.xml");

				var manifestDirectoryPath = Path.GetDirectoryName(manifestPath);
				if (!Directory.Exists(manifestDirectoryPath))
				{
					Directory.CreateDirectory(manifestDirectoryPath);
				}

				File.Copy(backupManifestPath, manifestPath);
			}

			var manifest = new AndroidManifestWrapper(manifestPath);

			if (string.IsNullOrEmpty(XsollaSettings.DeepLinkRedirectUrl))
			{
				Debug.LogError("Redirect URL for Android deep linking is empty. Please check plugin settings.");
				return;
			}

			var redirectUrl = new Uri(XsollaSettings.DeepLinkRedirectUrl);

			var data = new DataNode(redirectUrl.Scheme, redirectUrl.Host, redirectUrl.PathAndQuery);
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