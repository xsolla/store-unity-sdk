using UnityEditor;
using UnityEngine;

namespace Xsolla.Demo
{
	public partial class DemoSettingsEditor
	{
		private const string PLATFORM_LABEL = "Publishing Platform.";
		private const string PLATFORM_TOOLTIP = "Publishing platform the user plays on.";

		private const string PLATFORM_USERNAME_LABEL = "Username from Console";
		private const string PLATFORM_USERNAME_TOOLTIP = "Social username from console platform";

		private bool PlatformSettings()
		{
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical(GroupAreaStyle);
			EditorGUILayout.LabelField("Platform", GroupHeaderStyle);

			var useSteam = DemoSettings.UseSteamAuth;
			if (useSteam)
				GUI.enabled = false;

			var changed = false;

			var platform = (PlatformType) EditorGUILayout.EnumPopup(new GUIContent(PLATFORM_LABEL, PLATFORM_TOOLTIP), DemoSettings.Platform);
			if (platform != DemoSettings.Platform)
			{
				DemoSettings.Platform = platform;
				changed = true;
			}

			DemoSettings.UseConsoleAuth = !useSteam && platform > PlatformType.Xsolla;

			if (DemoSettings.UseConsoleAuth)
			{
				var username = EditorGUILayout.TextField(new GUIContent(PLATFORM_USERNAME_LABEL, PLATFORM_USERNAME_TOOLTIP), DemoSettings.UsernameFromConsolePlatform);
				if (username != DemoSettings.UsernameFromConsolePlatform)
				{
					DemoSettings.UsernameFromConsolePlatform = username;
					changed = true;
				}
			}

			if (useSteam)
				GUI.enabled = true;

			EditorGUILayout.EndVertical();
			return changed;
		}
	}
}