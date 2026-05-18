using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.DevTools
{
	public static class BuildsManager
	{
		public static void BuildTechDemo()
		{
			SdkSettingsManager.ResetSdkSettingsForTechDemo();
			BakeTestCredentials();

			var buildScenes = new[] {
				"Assets/TechDemo/Scenes/TechDemo.unity"
			};

			BuildProject(buildScenes);
		}

		private static void BakeTestCredentials()
		{
			var json = Environment.GetEnvironmentVariable("XSOLLA_TEST_CREDENTIALS");
			if (string.IsNullOrEmpty(json))
			{
				Debug.Log("[BuildsManager] XSOLLA_TEST_CREDENTIALS not set, skipping credentials bake");
				return;
			}

			var dir = Path.Combine(Application.dataPath, "Resources");
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			var filePath = Path.Combine(dir, "test-credentials-local.json");
			File.WriteAllText(filePath, json);
			Debug.Log($"[BuildsManager] Baked test credentials to {filePath}");
		}

		public static void BuildXsollaDemo()
		{
			SdkSettingsManager.ResetSdkSettingsForXsollaDemo();

			var buildScenes = new[] {
				"Assets/Xsolla.Demo/Scenes/XsollaDemo.unity"
			};

			BuildProject(buildScenes);
		}

		private static void UpdateProjectId(string id)
		{
			XsollaSettings.StoreProjectId = id;
			EditorUtility.SetDirty(XsollaSettings.Instance);
			AssetDatabase.SaveAssets();
		}

		private static void BuildProject(string[] buildScenes)
		{
			var buildTarget = GetBuildTarget();
			var buildPath = GetBuildPath(buildTarget);

			Debug.Log($"[BuildsManager] Begin. Target: {buildTarget} Path:{buildPath}");

			var buildPlayerOptions = new BuildPlayerOptions {
				scenes = buildScenes,
				locationPathName = buildPath,
				target = buildTarget,
				options = BuildOptions.None
			};

			if (buildTarget == BuildTarget.iOS)
				SetupIosSigningSettings();

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

		private static void SetupIosSigningSettings()
		{
			PlayerSettings.iOS.appleEnableAutomaticSigning = false;
			PlayerSettings.iOS.iOSManualProvisioningProfileType = ProvisioningProfileType.Distribution;
			PlayerSettings.iOS.appleDeveloperTeamID = GetEnvironmentArgument("IOS_SIGN_TEAM_ID");
			PlayerSettings.iOS.iOSManualProvisioningProfileID = GetEnvironmentArgument("IOS_PROVISION_ID");
		}

		private static BuildTarget GetBuildTarget()
		{
			var envArg = GetEnvironmentArgument("buildTarget");
			if (Enum.TryParse(envArg, out BuildTarget target))
				return target;

			throw new Exception($"Can't parse build target from environment argument: {envArg}");
		}

		private static string GetBuildPath(BuildTarget buildTarget)
		{
			var path = GetEnvironmentArgument("customBuildPath");
			var productName = Application.productName;

			switch (buildTarget)
			{
				case BuildTarget.Android:
					return Path.Combine(path, $"{productName}.apk");
				case BuildTarget.StandaloneWindows:
				case BuildTarget.StandaloneWindows64:
					return Path.Combine(path, $"{productName}.exe");
				case BuildTarget.StandaloneOSX:
					return Path.Combine(path, $"{productName}.app");
				default:
					return path;
			}
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

			var envVar = Environment.GetEnvironmentVariable(name);
			if (!string.IsNullOrWhiteSpace(envVar))
				return envVar;

			throw new Exception($"Can't parse environment argument: {name}");
		}
	}
}