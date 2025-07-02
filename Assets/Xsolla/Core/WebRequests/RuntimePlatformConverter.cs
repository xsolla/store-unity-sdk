using UnityEngine;

namespace Xsolla.Core
{
	internal static class RuntimePlatformConverter
	{
		public static string GetRunningPlatformAsString()
		{
			var platform = Application.platform;

#pragma warning disable CS0618 // Type or member is obsolete
			return platform switch {
				RuntimePlatform.OSXEditor          => "osxeditor",
				RuntimePlatform.OSXPlayer          => "osxplayer",
				RuntimePlatform.WindowsPlayer      => "windowsplayer",
				RuntimePlatform.WindowsEditor      => "windowseditor",
				RuntimePlatform.IPhonePlayer       => "iphoneplayer",
				RuntimePlatform.XBOX360            => "xbox360",
				RuntimePlatform.PS3                => "ps3",
				RuntimePlatform.Android            => "android",
				RuntimePlatform.NaCl               => "nacl",
				RuntimePlatform.FlashPlayer        => "flashplayer",
				RuntimePlatform.LinuxPlayer        => "linuxplayer",
				RuntimePlatform.LinuxEditor        => "linuxeditor",
				RuntimePlatform.WebGLPlayer        => "webglplayer",
				RuntimePlatform.MetroPlayerX86     => "metroplayerx86",
				RuntimePlatform.MetroPlayerX64     => "metroplayerx64",
				RuntimePlatform.MetroPlayerARM     => "metroplayerarm",
				RuntimePlatform.WP8Player          => "wp8player",
				RuntimePlatform.BB10Player         => "bb10player",
				RuntimePlatform.TizenPlayer        => "tizenplayer",
				RuntimePlatform.PSP2               => "psp2",
				RuntimePlatform.PS4                => "ps4",
				RuntimePlatform.PSM                => "psm",
				RuntimePlatform.XboxOne            => "xboxone",
				RuntimePlatform.SamsungTVPlayer    => "samsungtvplayer",
				RuntimePlatform.WiiU               => "wiiu",
				RuntimePlatform.tvOS               => "tvos",
				RuntimePlatform.Switch             => "switch",
				RuntimePlatform.Lumin              => "lumin",
				RuntimePlatform.Stadia             => "stadia",
				RuntimePlatform.CloudRendering     => "cloudrendering",
				RuntimePlatform.GameCoreScarlett   => "gamecorescarlett",
				RuntimePlatform.GameCoreXboxSeries => "gamecorexboxseries",
				RuntimePlatform.GameCoreXboxOne    => "gamecorexboxone",
				RuntimePlatform.PS5                => "ps5",
				RuntimePlatform.EmbeddedLinuxArm64 => "embeddedlinuxarm64",
				RuntimePlatform.EmbeddedLinuxArm32 => "embeddedlinuxarm32",
				RuntimePlatform.EmbeddedLinuxX64   => "embeddedlinuxx64",
				RuntimePlatform.EmbeddedLinuxX86   => "embeddedlinuxx86",
				RuntimePlatform.LinuxServer        => "linuxserver",
				RuntimePlatform.WindowsServer      => "windowsserver",
				RuntimePlatform.OSXServer          => "osxserver",
				_                                  => platform.ToString().ToLowerInvariant()
			};
#pragma warning restore CS0618 // Type or member is obsolete
		}
	}
}