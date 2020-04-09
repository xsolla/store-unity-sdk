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

		private static bool m_useConsoleCheckbox;
		
		private void Awake()
		{
			m_useConsoleCheckbox = false;
			if (XsollaSettings.UseSteamAuth)
			{
				XsollaSettings.UseConsoleAuth = false;
			}
			else
			{
				XsollaSettings.UseConsoleAuth = 
					(XsollaSettings.Platform != PlatformType.None) && 
					(XsollaSettings.Platform != PlatformType.Xsolla);
			}
		}

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
			XsollaSettings.Platform = PlatformType.Other;
			XsollaSettings.UseConsoleAuth = false;
			m_useConsoleCheckbox = true;
		}

		/// <summary>
		/// Console platform settings.
		/// </summary>
		private static void ConsoleSettings()
		{
			if (m_useConsoleCheckbox)
			{
				XsollaSettings.UseConsoleAuth = EditorGUILayout.Toggle("Use console auth", XsollaSettings.UseConsoleAuth);
				if (XsollaSettings.UseConsoleAuth)
				{
					XsollaSettings.Platform = PlatformType.Xsolla;
					XsollaSettings.UseSteamAuth = false;
					m_useConsoleCheckbox = false;
				}
			}
			else
			{
				if (XsollaSettings.UseSteamAuth) return;
				XsollaSettings.Platform = (PlatformType)EditorGUILayout.EnumPopup(new GUIContent("Publishing platform", PLATFORM_TOOLTIP), XsollaSettings.Platform);
				if ((XsollaSettings.Platform != PlatformType.None) && (XsollaSettings.Platform != PlatformType.Xsolla))
				{
					XsollaSettings.UsernameFromConsolePlatform = EditorGUILayout.TextField(new GUIContent("Username from console", PLATFORM_USERNAME_TOOLTIP), XsollaSettings.UsernameFromConsolePlatform);						
				}
				else
				{
					XsollaSettings.UseConsoleAuth = false;
				}
			}
		}
	}
}

