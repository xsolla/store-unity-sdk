using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Xsolla.Core.Browser
{
	public class XsollaBrowserCheckPreprocessor : IPreprocessBuildWithReport
	{
		public int callbackOrder { get; }

		public void OnPreprocessBuild(BuildReport report)
		{
			if (report.summary.platformGroup != BuildTargetGroup.Standalone || !XsollaSettings.InAppBrowserEnabled)
				return;

			/*TEXTREVIEW*/
			//Check IL2CPP - browser support only Mono
			if (PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup) == ScriptingImplementation.IL2CPP)
				Debug.LogWarning(@"WARNING: In-App Browser does not support IL2CPP scripting backend and will result in Win32Exception upon launch.
				Please change scripting backend to Mono or disable In-App Browser.");
		}
	}
}