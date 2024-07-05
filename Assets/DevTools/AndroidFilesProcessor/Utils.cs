#if UNITY_ANDROID
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Xsolla.DevTools
{
	public static class Utils
	{
		public static string GetWorkDir()
		{
			var guids = AssetDatabase.FindAssets($"t:Script {nameof(Utils)}");
			if (guids.Length == 0)
				throw new FileNotFoundException($"Can't find {nameof(Utils)} script");

			var path = AssetDatabase.GUIDToAssetPath(guids[0]);
			path = path.Replace("Assets", Application.dataPath);
			path = Path.GetDirectoryName(path);

			if (path == null)
				throw new DirectoryNotFoundException("Can't find directory with android file templates");

			return path;
		}

		public static string GetTargetDir()
		{
			return Path.Combine(Application.dataPath, "Plugins", "Android");
		}

		public static void CopyFileIfNotExists(string sourcePath, string targetPath)
		{
			if (!File.Exists(sourcePath))
				throw new FileNotFoundException($"File not found: {sourcePath}");

			if (File.Exists(targetPath))
				return;

			var targetDir = Path.GetDirectoryName(targetPath);
			if (targetDir != null && !Directory.Exists(targetDir))
				Directory.CreateDirectory(targetDir);

			File.Copy(sourcePath, targetPath);
		}
	}
}
#endif