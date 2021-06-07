using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Xsolla
{
	public static class BuildsManager
	{
		public static void PerformBuild()
		{
			var buildPlayerOptions = new BuildPlayerOptions
			{
				scenes = GetBuildScenes(),
				locationPathName = GetEnvironmentArgument("buildPath"),
				target = GetBuildTargetByArgument(),
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
		}

		private static BuildTarget GetBuildTargetByArgument()
		{
			var arg = GetEnvironmentArgument("buildTarget");
			if (string.IsNullOrEmpty(arg))
				throw new Exception("Build target argument not found");

			if (arg == "WebGL")
				return BuildTarget.WebGL;

			throw new Exception($"Unexpected build target argument: {arg}");
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

			return null;
		}

		private static string[] GetBuildScenes()
		{
			return EditorBuildSettings.scenes
				.Where(scene => scene.enabled)
				.Where(scene => !string.IsNullOrEmpty(scene.path))
				.Select(sce => sce.path)
				.ToArray();
		}
	}
}