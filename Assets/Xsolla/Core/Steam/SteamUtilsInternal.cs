#if UNITY_STANDALONE
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xsolla.Core
{
	internal static class SteamUtilsInternal
	{
		public static bool IsSteamworksAvailable()
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