using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Xsolla.XsollaBrowser
{
	public class BrowserPrefabProcessor : IPreprocessBuildWithReport
	{
		public int callbackOrder { get; }

		private static string SavedOriginPrefabPath;

		public void OnPreprocessBuild(BuildReport report)
		{
			SavedOriginPrefabPath = null;

			if (report.summary.platformGroup == BuildTargetGroup.Standalone)
				return;

			EditorApplication.update += OnBuildFinish;

			var originPrefabPath = GetOriginPrefabPath();
			var tempPrefabPath = GetTempPrefabPath();

			var error = AssetDatabase.MoveAsset(originPrefabPath, tempPrefabPath);
			if (!string.IsNullOrEmpty(error))
			{
				Debug.LogError($"Can't move {originPrefabPath} to {tempPrefabPath}: {error}");
				return;
			}

			AssetDatabase.Refresh();
			SavedOriginPrefabPath = originPrefabPath;
		}

		private void OnBuildFinish()
		{
			EditorApplication.update -= OnBuildFinish;

			if (SavedOriginPrefabPath == null)
				return;

			var originPrefabPath = SavedOriginPrefabPath;
			var tempPrefabPath = GetTempPrefabPath();

			var error = AssetDatabase.MoveAsset(tempPrefabPath, originPrefabPath);
			if (!string.IsNullOrEmpty(error))
				Debug.LogError($"Can't move {tempPrefabPath} to {originPrefabPath}: {error}");

			AssetDatabase.Refresh();
			SavedOriginPrefabPath = null;
		}

		private static string GetOriginPrefabPath()
		{
			var guids = AssetDatabase.FindAssets("t:Prefab XsollaWebBrowser");
			if (guids.Length == 0)
				throw new FileNotFoundException($"Can't find {nameof(BrowserPrefabProcessor)} script");

			if (guids.Length > 1)
				throw new FileNotFoundException($"Found more than one {nameof(BrowserPrefabProcessor)} script");

			return AssetDatabase.GUIDToAssetPath(guids[0]);
		}

		private static string GetTempPrefabPath()
		{
			return Path.Combine("Assets", "XsollaWebBrowser.prefab");
		}
	}
}