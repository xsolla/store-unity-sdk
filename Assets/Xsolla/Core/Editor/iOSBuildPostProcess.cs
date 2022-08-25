#if UNITY_IOS

using System;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;

namespace Xsolla.Core.Editor
{
	public class iOSBuildPostProcess : IPostprocessBuildWithReport
	{
		public int callbackOrder => 2000;
		
		public void OnPostprocessBuild(BuildReport report)
		{
			Debug.Log("Xsolla iOSBuildPostProcess is now postprocessing iOS Project");
			SetupXcodeProject(report.summary.outputPath);
			SetupDeepLinking(report.summary.outputPath);
		}

		private void SetupXcodeProject(string outputPath)
		{
			var projectPath = PBXProject.GetPBXProjectPath(outputPath);
			var project = new PBXProject();
			project.ReadFromFile(projectPath);

#if UNITY_2019_3_OR_NEWER
			var targetGuid = project.GetUnityFrameworkTargetGuid();
#else
			var targetName = PBXProject.GetUnityTargetName();
			var targetGuid = project.TargetGuidByName(targetName);
#endif
			
			project.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");

			try
			{
				var projectInString = File.ReadAllText(projectPath);
				projectInString = projectInString.Replace("ENABLE_BITCODE = YES;",$"ENABLE_BITCODE = NO;");
				File.WriteAllText(projectPath, projectInString);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			project.WriteToFile(projectPath);
		}
		
		private void SetupDeepLinking(string outputPath)
		{
			var plistPath = Path.Combine(outputPath, "Info.plist");
			var plistDoc = new PlistDocument();
			plistDoc.ReadFromString(File.ReadAllText(plistPath));
       
			var rootDic = plistDoc.root;
			var urlTypesArray = rootDic.CreateArray("CFBundleURLTypes");
			
			var urlNamesDic = urlTypesArray.AddDict();
			urlNamesDic.SetString("CFBundleURLName", "");
			
			var urlSchemesArray = urlNamesDic.CreateArray("CFBundleURLSchemes");
			urlSchemesArray.AddString("app");
			
			File.WriteAllText(plistPath, plistDoc.WriteToString());
		}
	}
}

#endif
