#if UNITY_ANDROID
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Xsolla.DevTools
{
	public class AndroidFilesProcessor : IPreprocessBuildWithReport
	{
		public int callbackOrder => -1000;

		public void OnPreprocessBuild(BuildReport report)
		{
			ClearTargetDir();

			GradlePathProcessor.Process();
			ManifestProcessor.Process();

			GradleTemplateProcessor.Process();
			BaseProjectTemplateProcessor.Process();
			MainTemplateProcessor.Process();
			LauncherTemplateProcessor.Process();
		}

		private void ClearTargetDir()
		{
			var targetDir = Utils.GetTargetDir();
			if (Directory.Exists(targetDir))
				Directory.Delete(targetDir, true);
		}
	}
}
#endif