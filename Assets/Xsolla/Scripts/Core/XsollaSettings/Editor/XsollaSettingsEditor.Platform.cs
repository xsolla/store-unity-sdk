using System;
using UnityEditor;
using UnityEngine;
using Xsolla.Store;

namespace Xsolla.Core
{
	public partial class XsollaSettingsEditor : UnityEditor.Editor
	{
		private const string STEAM_AUTH_TOOLTIP = "If enabled, Login try find Steam client and get `session_ticket`." +
		                                        "Then this ticket will be changed to JWT.";

		private const string PLATFORM_TOOLTIP = "Publishing platform the user plays on.";
		private const string PLATFORM_USERNAME_TOOLTIP = "Social username from console platform";

		private static void PublishingPlatformSettings()
		{
			using (new EditorGUILayout.VerticalScope("box"))
			{
				GUILayout.Label("Publishing Platform Settings", EditorStyles.boldLabel);
				SteamSettings();
				ConsoleSettings();
			}
			EditorGUILayout.Space();
		}

		/// <summary>
		/// Steam platform settings.
		/// </summary>
		private static void SteamSettings()
		{
			XsollaSettings.UseSteamAuth = EditorGUILayout.Toggle(new GUIContent("Use Steam authorization?", STEAM_AUTH_TOOLTIP), XsollaSettings.UseSteamAuth);
			if (!XsollaSettings.UseSteamAuth) return;
			
			XsollaSettings.SteamAppId = EditorGUILayout.TextField("Steam App ID", XsollaSettings.SteamAppId);
			XsollaSettings.Platform = PlatformType.PC_Other;
			XsollaSettings.UseConsoleAuth = false;
		}

		/// <summary>
		/// Console platform settings.
		/// </summary>
		private static void ConsoleSettings()
		{
			XsollaSettings.Platform = (PlatformType)EditorGUILayout.EnumPopup(new GUIContent("Publishing platform", PLATFORM_TOOLTIP), XsollaSettings.Platform);
			
			if ((XsollaSettings.Platform == PlatformType.PC_Other) && XsollaSettings.UseSteamAuth) return;
			XsollaSettings.UseSteamAuth = false;
			
			if ((XsollaSettings.Platform != PlatformType.None) && (XsollaSettings.Platform != PlatformType.Xsolla))
			{
				XsollaSettings.UsernameFromConsolePlatform = EditorGUILayout.TextField(new GUIContent("Username from console", PLATFORM_USERNAME_TOOLTIP), XsollaSettings.UsernameFromConsolePlatform);
				XsollaSettings.UseConsoleAuth = true;
			}
			else
				XsollaSettings.UseConsoleAuth = false;
		}
	}
}

