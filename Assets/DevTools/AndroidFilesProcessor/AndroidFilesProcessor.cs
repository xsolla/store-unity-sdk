#if UNITY_ANDROID
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Xsolla.DevTools
{
	public class AndroidFilesProcessor : IPreprocessBuildWithReport
	{
		public int callbackOrder => -1000;

		public void OnPreprocessBuild(BuildReport report)
		{
			Utils.ClearTargetDir();

			ApiLevelProcessor.Process();
			GradlePathProcessor.Process();

			ManifestProcessor.Process();

			GradleTemplateProcessor.Process();
			BaseProjectTemplateProcessor.Process();
			MainTemplateProcessor.Process();
			LauncherTemplateProcessor.Process();
		}
	}
}
#endif