using UnityEditor;
using UnityEngine;

namespace Xsolla.Core
{
	public partial class XsollaSettingsEditor : UnityEditor.Editor
	{
		private const string STEAM_AUTH_TOOLTIP = "If enabled, Login try find Steam client and get `session_ticket`." +
												"Then this ticket will be changed to JWT.";

		private const string PLATFORM_TOOLTIP = "Publishing platform the user plays on.";
		private const string PLATFORM_USERNAME_TOOLTIP = "Social username from console platform";

		private static bool PublishingPlatformSettings()
		{
			bool changed;
			using (new EditorGUILayout.VerticalScope("box"))
			{
				GUILayout.Label("Publishing Platform Settings", EditorStyles.boldLabel);
				changed = SteamSettings();
				changed = changed || ConsoleSettings();
				
			}
			EditorGUILayout.Space();

			return changed;
		}

		/// <summary>
		/// Steam platform settings.
		/// </summary>
		private static bool SteamSettings()
		{
			var changed = false;
			var useSteam = EditorGUILayout.Toggle(new GUIContent("Use Steam authorization?", STEAM_AUTH_TOOLTIP), XsollaSettings.UseSteamAuth);
			if (useSteam != XsollaSettings.UseSteamAuth)
			{
				XsollaSettings.UseSteamAuth = useSteam;
				changed = true;
			}
			if (!XsollaSettings.UseSteamAuth) return changed;
			
			var appId = EditorGUILayout.TextField("Steam App ID", XsollaSettings.SteamAppId);
			if (appId != XsollaSettings.SteamAppId)
			{
				XsollaSettings.SteamAppId = appId;
				changed = true;
			}
			XsollaSettings.Platform = PlatformType.PC_Other;
			XsollaSettings.UseConsoleAuth = false;

			return changed;
		}

		/// <summary>
		/// Console platform settings.
		/// </summary>
		private static bool ConsoleSettings()
		{
			var changed = false;
			var platform = (PlatformType)EditorGUILayout.EnumPopup(new GUIContent("Publishing platform", PLATFORM_TOOLTIP), XsollaSettings.Platform);
			if (platform != XsollaSettings.Platform)
			{
				XsollaSettings.Platform = platform;
				changed = true;
			}
			
			if ((XsollaSettings.Platform == PlatformType.PC_Other) && XsollaSettings.UseSteamAuth) return changed;
			XsollaSettings.UseSteamAuth = false;
			
			if ((XsollaSettings.Platform != PlatformType.None) && (XsollaSettings.Platform != PlatformType.Xsolla))
			{
				var username = EditorGUILayout.TextField(new GUIContent("Username from console", PLATFORM_USERNAME_TOOLTIP), XsollaSettings.UsernameFromConsolePlatform);
				if (username != XsollaSettings.UsernameFromConsolePlatform)
				{
					XsollaSettings.UsernameFromConsolePlatform = username;
					changed = true;
				}
				XsollaSettings.UseConsoleAuth = true;
			}
			else
				XsollaSettings.UseConsoleAuth = false;
			return changed;
		}
	}
}
