using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Xsolla.Core.Editor
{
	public partial class XsollaSettingsEditor : UnityEditor.Editor
	{
		private const string STEAM_AUTH_TOOLTIP = "If enabled, Login try find Steam client and get `session_ticket`." +
												"Then this ticket will be changed to JWT.";

		private const string PLATFORM_TOOLTIP = "Publishing platform the user plays on.";
		
		private const string PLATFORM_USERNAME_TOOLTIP = "Social username from console platform";

		private const string STEAM_APP_ID_TOOLTIP = "You will need to restart Unity Editor after changing Steam App ID";

		private static PlatformType _prevPlatform = PlatformType.Xsolla;

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

				if (useSteam)
					_prevPlatform = XsollaSettings.Platform;
				else
					XsollaSettings.Platform = _prevPlatform;
			}

			if (!XsollaSettings.UseSteamAuth)
				return changed;

			var appId = EditorGUILayout.TextField(new GUIContent("Steam App Id [!]", STEAM_APP_ID_TOOLTIP), XsollaSettings.SteamAppId);
			if (appId != XsollaSettings.SteamAppId)
			{
				XsollaSettings.SteamAppId = appId;
				changed = true;
				
				var steamAppIdFiles = new []
				{
					Application.dataPath.Replace("Assets", "steam_appid.txt"),
					Path.Combine(Application.dataPath, "Plugins", "Steamworks.NET", "redist", "steam_appid.txt")
				};
				
				try
				{
					foreach (var filePath in steamAppIdFiles)
					{
						File.WriteAllText(filePath, appId);
					}
				
					Debug.Log("[Steamworks.NET] Files \"steam_appid.txt\" successfully updated. Please relaunch Unity");
				}
				catch (Exception)
				{
					var filePathsArray = string.Join("\n", steamAppIdFiles);
					Debug.LogWarning($"[Steamworks.NET] Could not write SteamAppId in the files \"steam_appid.txt\". Please edit them manually and relaunch Unity. Files:\n{filePathsArray}\n");
				}
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
