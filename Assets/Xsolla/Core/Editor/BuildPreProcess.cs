using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Xsolla.Core
{
	public class BuildPreProcess : IPreprocessBuildWithReport
	{
		public int callbackOrder
		{
			get { return 3000; }
		}

		public void OnPreprocessBuild(BuildReport report)
		{
			if (report.summary.platform == BuildTarget.StandaloneOSX)
			{
				var steamLibPath = "Assets/Xsolla/ThirdParty/Steamworks.NET-master/Plugins/steam_api.bundle";
				var steamLibAsset = AssetImporter.GetAtPath(steamLibPath) as PluginImporter;
				if (steamLibAsset != null)
				{
					steamLibAsset.SetPlatformData(BuildTarget.StandaloneOSX,"OS", "OSX");
					steamLibAsset.SetPlatformData(BuildTarget.StandaloneOSX,"CPU", "AnyCPU");
					steamLibAsset.SaveAndReimport();
				}
			}
		}
	}
}