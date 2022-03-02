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

		private void PlatformSettings()
		{
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical(GroupAreaStyle);
			EditorGUILayout.LabelField("Platform", GroupHeaderStyle);

			GUI.enabled = !DemoSettings.UseSteamAuth;

			var platform = (PlatformType) EditorGUILayout.EnumPopup(new GUIContent(PLATFORM_LABEL, PLATFORM_TOOLTIP), DemoSettings.Platform);
			if (platform != DemoSettings.Platform)
			{
				DemoSettings.Platform = platform;
			}

			GUI.enabled = true;

			if (DemoSettings.UseSteamAuth)
			{
				EditorGUILayout.EndVertical();
				return;
			}

			if (DemoSettings.Platform != PlatformType.None && DemoSettings.Platform != PlatformType.Xsolla)
			{
				var username = EditorGUILayout.TextField(new GUIContent(PLATFORM_USERNAME_LABEL, PLATFORM_USERNAME_TOOLTIP), DemoSettings.UsernameFromConsolePlatform);
				if (username != DemoSettings.UsernameFromConsolePlatform)
				{
					DemoSettings.UsernameFromConsolePlatform = username;
				}

				DemoSettings.UseConsoleAuth = true;
			}
			else
			{
				DemoSettings.UseConsoleAuth = false;
			}

			EditorGUILayout.EndVertical();
		}
	}
}