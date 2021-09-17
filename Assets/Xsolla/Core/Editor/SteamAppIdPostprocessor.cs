using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Xsolla.Core
{
	public class SteamAppIdPostprocessor : IPostprocessBuildWithReport
	{
		public int callbackOrder { get; }

		public void OnPostprocessBuild(BuildReport report)
		{
			if (report.summary.platformGroup != BuildTargetGroup.Standalone)
				return;

			if (!XsollaSettings.UseSteamAuth)
				return;

			var filePath = Path.GetDirectoryName(report.summary.outputPath);
			filePath = Path.Combine(filePath, "steam_appid.txt");
			File.WriteAllText(filePath, XsollaSettings.SteamAppId);
		}
	}
}