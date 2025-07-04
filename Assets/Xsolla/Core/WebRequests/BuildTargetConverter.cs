#if UNITY_EDITOR
using UnityEditor;

namespace Xsolla.Core
{
	internal static class BuildTargetConverter
	{
		public static string GetActiveBuildTargetAsString()
		{
			var target = EditorUserBuildSettings.activeBuildTarget;

			return target switch {
				BuildTarget.StandaloneOSX            => "standaloneosx",
				BuildTarget.StandaloneWindows        => "standalonewindows",
				BuildTarget.StandaloneWindows64      => "standalonewindows64",
				BuildTarget.StandaloneLinux64        => "standalonelinux64",
				BuildTarget.WebGL                    => "webgl",
				BuildTarget.WSAPlayer                => "wsaplayer",
				BuildTarget.Android                  => "android",
				BuildTarget.iOS                      => "ios",
				BuildTarget.PS4                      => "ps4",
				BuildTarget.XboxOne                  => "xboxone",
				BuildTarget.tvOS                     => "tvos",
				BuildTarget.Switch                   => "switch",
				BuildTarget.GameCoreXboxOne          => "gamecorexboxone",
				BuildTarget.PS5                      => "ps5",
				BuildTarget.EmbeddedLinux            => "embeddedlinux",
				BuildTarget.NoTarget                 => "notarget",
				_                                    => target.ToString().ToLowerInvariant()
			};
		}
	}
}
#endif