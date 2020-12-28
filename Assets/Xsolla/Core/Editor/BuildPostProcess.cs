#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Xsolla.Core
{
	public class BuildPostProcess : IPostprocessBuildWithReport
	{
		public int callbackOrder { get; }

		public void OnPostprocessBuild(BuildReport report)
		{
			AddSteamAppIdFile(report);
		}

		private void AddSteamAppIdFile(BuildReport report)
		{
			if (!XsollaSettings.UseSteamAuth)
				return;

			if (report.summary.platformGroup != BuildTargetGroup.Standalone)
				return;

			var filePath = Path.GetDirectoryName(report.summary.outputPath);
			filePath = Path.Combine(filePath, "steam_appid.txt");
			File.WriteAllText(filePath, XsollaSettings.SteamAppId);
		}
	}
}

#endif