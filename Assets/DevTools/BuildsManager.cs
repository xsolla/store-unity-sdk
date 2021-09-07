using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Xsolla.DevTools
{
	public static class BuildsManager
	{
		public static void Build()
		{
			var buildTarget = GetBuildTarget();
			var buildPath = GetBuildPath(buildTarget);
			var buildScenes = GetBuildScenes();

			Debug.Log($"[BuildsManager] Begin. Target: {buildTarget} Path:{buildPath}");

			var buildPlayerOptions = new BuildPlayerOptions{
				scenes = buildScenes,
				locationPathName = buildPath,
				target = buildTarget,
				options = BuildOptions.None
			};

			var buildResult = BuildPipeline.BuildPlayer(buildPlayerOptions).summary.result;
			switch (buildResult)
			{
				case BuildResult.Succeeded:
					Debug.Log("[BuildsManager] Build succeeded");
					break;
				case BuildResult.Failed:
					Debug.Log("[BuildsManager] Build failed");
					break;
				case BuildResult.Cancelled:
					Debug.Log("[BuildsManager] Build cancelled");
					break;
				case BuildResult.Unknown:
					Debug.Log("[BuildsManager] Unknown error");
					break;
				default:
					throw new ArgumentOutOfRangeException(buildResult.ToString());
			}

			var exitCode = buildResult == BuildResult.Succeeded ? 0 : 1;
			EditorApplication.Exit(exitCode);
		}

		private static BuildTarget GetBuildTarget()
		{
			var envArg = GetEnvironmentArgument("customBuildTarget");
			if (Enum.TryParse(envArg, out BuildTarget target))
			{
				return target;
			}

			throw new Exception($"Can't parse build target from environment argument: {envArg}");
		}

		private static string GetBuildPath(BuildTarget target)
		{
			var path = GetEnvironmentArgument("customBuildPath");
			var productName = Application.productName;

			switch (target)
			{
				case BuildTarget.Android:
					return Path.Combine(path, $"{productName}.apk");
				case BuildTarget.StandaloneWindows:
				case BuildTarget.StandaloneWindows64:
					return Path.Combine(path, $"{productName}.exe");
				default:
					return path;
			}
		}

		private static string[] GetBuildScenes()
		{
			return EditorBuildSettings.scenes
				.Where(scene => scene.enabled)
				.Where(scene => !string.IsNullOrEmpty(scene.path))
				.Select(sce => sce.path)
				.ToArray();
		}

		private static string GetEnvironmentArgument(string name)
		{
			var args = Environment.GetCommandLineArgs();
			for (var i = 0; i < args.Length; i++)
			{
				if (args[i].Contains(name))
				{
					return args[i + 1];
				}
			}

			throw new Exception($"Can't parse environment argument: {name}");
		}
	}
}