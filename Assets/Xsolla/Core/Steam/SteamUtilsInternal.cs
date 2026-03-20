#if UNITY_STANDALONE
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xsolla.Core
{
	internal static class SteamUtilsInternal
	{
		private static bool IsSteamworksAvailable()
		{
			return GetSteamAPIType() != null;
		}

		public static bool TryInit()
		{
			if (!IsSteamworksAvailable())
				return false;

			var steamApiType = GetSteamAPIType();
			if (steamApiType == null)
				return false;

			var initMethod = steamApiType.GetMethod("Init", BindingFlags.Public | BindingFlags.Static);
			if (initMethod == null)
				return false;

			object result = initMethod.Invoke(null, null);
			return result is bool b && b;
		}

		public static string GetSteamSessionTicket()
		{
			if (!IsSteamworksAvailable())
				return null;

			var ticket = TryGetAuthSessionTicketData();
			if (ticket == null || ticket.Length == 0)
			{
				XDebug.LogError("Get steam session ticket failed: Ticket is null or empty.");
				return null;
			}

			return ConvertTicket(ticket);
		}

		public static void OpenSteamOverlay(string url)
		{
			if (!IsSteamworksAvailable())
			{
				XDebug.LogWarning("Steamworks.NET not found. Cannot open Steam overlay.");
				return;
			}

			if (string.IsNullOrEmpty(url))
			{
				XDebug.LogWarning("Cannot open Steam overlay: url is null or empty.");
				return;
			}

			var steamFriendsType = GetTypeFromAllAssemblies("Steamworks.SteamFriends");
			if (steamFriendsType == null)
			{
				XDebug.LogWarning("SteamFriends type not found.");
				return;
			}

			var methods = steamFriendsType
				.GetMethods(BindingFlags.Public | BindingFlags.Static)
				.Where(m => m.Name == "ActivateGameOverlayToWebPage")
				.ToArray();

			if (methods.Length == 0)
			{
				XDebug.LogWarning("ActivateGameOverlayToWebPage method not found.");
				return;
			}

			foreach (var m in methods)
			{
				var args = string.Join(", ", m.GetParameters().Select(p => $"{p.ParameterType.FullName} {p.Name}"));
				XDebug.Log($"Found method: {m.Name}({args})");
			}

			var method = methods.First();
			var parameters = method.GetParameters();

			try
			{
				if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string))
				{
					method.Invoke(null, new object[] { url });
				}
				else if (parameters.Length == 2 && parameters[0].ParameterType == typeof(string))
				{
					var modeType = parameters[1].ParameterType;

					// Пытаемся взять enum value "...Default"
					object defaultMode = Enum.GetValues(modeType).GetValue(0);

					var namedDefault = Enum
						.GetNames(modeType)
						.FirstOrDefault(n => n.IndexOf("Default", StringComparison.OrdinalIgnoreCase) >= 0);

					if (namedDefault != null)
						defaultMode = Enum.Parse(modeType, namedDefault);

					method.Invoke(null, new object[] { url, defaultMode });
				}
				else
				{
					XDebug.LogWarning("Unsupported ActivateGameOverlayToWebPage signature.");
					return;
				}

				XDebug.Log($"Trying to open Steam overlay web page: {url}");
			}
			catch (Exception e)
			{
				XDebug.LogError($"Failed to open Steam overlay web page: {e}");
			}
		}

		private static Type GetSteamAPIType()
		{
			return AppDomain.CurrentDomain
				.GetAssemblies()
				.Select(asm => asm.GetType("Steamworks.SteamAPI"))
				.FirstOrDefault(t => t != null);
		}

		private static byte[] TryGetAuthSessionTicketData()
		{
			var steamUserType = GetTypeFromAllAssemblies("Steamworks.SteamUser");
			var identityType = GetTypeFromAllAssemblies("Steamworks.SteamNetworkingIdentity");

			if (steamUserType == null || identityType == null)
			{
				XDebug.LogWarning("Steamworks.NET not found.");
				return null;
			}

			object identityInstance = Activator.CreateInstance(identityType);
			byte[] ticket = new byte[1024];

			object[] parameters = new object[4];
			parameters[0] = ticket;
			parameters[1] = 1024;
			parameters[2] = 0u; // out uint length
			parameters[3] = identityInstance;

			MethodInfo getAuthMethod = steamUserType.GetMethod(
				"GetAuthSessionTicket",
				BindingFlags.Public | BindingFlags.Static
				);

			if (getAuthMethod == null)
			{
				XDebug.LogWarning("GetAuthSessionTicket method not found.");
				return null;
			}

			object result = getAuthMethod.Invoke(null, parameters);
			uint length = Convert.ToUInt32(parameters[2]);
			Array.Resize(ref ticket, (int) length);
			return ticket;
		}

		private static string ConvertTicket(byte[] ticket)
		{
			var stringBuilder = new StringBuilder();
			ticket.ToList().ForEach(b => stringBuilder.AppendFormat("{0:x2}", b));
			return stringBuilder.ToString();
		}

		private static Type GetTypeFromAllAssemblies(string fullTypeName)
		{
			return AppDomain.CurrentDomain
				.GetAssemblies()
				.Select(a => a.GetType(fullTypeName))
				.FirstOrDefault(t => t != null);
		}
	}
}
#endif