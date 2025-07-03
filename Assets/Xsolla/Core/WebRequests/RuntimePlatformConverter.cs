using UnityEngine;

namespace Xsolla.Core
{
	internal static class RuntimePlatformConverter
	{
		public static string GetRunningPlatformAsString()
		{
			var platform = Application.platform;

			return platform switch {
				RuntimePlatform.OSXEditor          => "osxeditor",
				RuntimePlatform.OSXPlayer          => "osxplayer",
				RuntimePlatform.WindowsPlayer      => "windowsplayer",
				RuntimePlatform.WindowsEditor      => "windowseditor",
				RuntimePlatform.IPhonePlayer       => "iphoneplayer",
				RuntimePlatform.Android            => "android",
				RuntimePlatform.LinuxPlayer        => "linuxplayer",
				RuntimePlatform.LinuxEditor        => "linuxeditor",
				RuntimePlatform.WebGLPlayer        => "webglplayer",
				RuntimePlatform.PS4                => "ps4",
				RuntimePlatform.XboxOne            => "xboxone",
				RuntimePlatform.tvOS               => "tvos",
				RuntimePlatform.Switch             => "switch",
				RuntimePlatform.PS5                => "ps5",
				_                                  => platform.ToString().ToLowerInvariant()
			};
		}
	}
}