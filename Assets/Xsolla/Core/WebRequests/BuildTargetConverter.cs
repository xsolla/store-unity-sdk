#if UNITY_EDITOR
using UnityEditor;

namespace Xsolla.Core
{
	internal static class BuildTargetConverter
	{
		public static string GetActiveBuildTargetAsString()
		{
			var target = EditorUserBuildSettings.activeBuildTarget;

#pragma warning disable CS0618 // Type or member is obsolete
			return target switch {
				BuildTarget.StandaloneOSX            => "standaloneosx",
				BuildTarget.StandaloneWindows        => "standalonewindows",
				BuildTarget.StandaloneWindows64      => "standalonewindows64",
				BuildTarget.StandaloneLinux          => "standalonelinux",
				BuildTarget.StandaloneLinux64        => "standalonelinux64",
				BuildTarget.StandaloneLinuxUniversal => "standalonelinuxuniversal",
				BuildTarget.WebGL                    => "webgl",
				BuildTarget.WSAPlayer                => "wsaplayer",
				BuildTarget.Android                  => "android",
				BuildTarget.iOS                      => "ios",
				BuildTarget.PS3                      => "ps3",
				BuildTarget.XBOX360                  => "xbox360",
				BuildTarget.WP8Player                => "wp8player",
				BuildTarget.StandaloneOSXIntel64     => "standaloneosxintel64",
				BuildTarget.BlackBerry               => "blackberry",
				BuildTarget.Tizen                    => "tizen",
				BuildTarget.PSP2                     => "psp2",
				BuildTarget.PS4                      => "ps4",
				BuildTarget.PSM                      => "psm",
				BuildTarget.XboxOne                  => "xboxone",
				BuildTarget.SamsungTV                => "samsungtv",
				BuildTarget.N3DS                     => "n3ds",
				BuildTarget.WiiU                     => "wiiu",
				BuildTarget.tvOS                     => "tvos",
				BuildTarget.Switch                   => "switch",
				BuildTarget.Lumin                    => "lumin",
				BuildTarget.Stadia                   => "stadia",
				BuildTarget.CloudRendering           => "cloudrendering",
				BuildTarget.GameCoreScarlett         => "gamecorescarlett",
				BuildTarget.GameCoreXboxOne          => "gamecorexboxone",
				BuildTarget.PS5                      => "ps5",
				BuildTarget.EmbeddedLinux            => "embeddedlinux",
				BuildTarget.NoTarget                 => "notarget",
				_                                    => target.ToString().ToLowerInvariant()
			};
#pragma warning restore CS0618 // Type or member is obsolete
		}
	}
}
#endif