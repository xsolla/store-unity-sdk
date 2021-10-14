using System;
using System.Linq;
using System.Text;
using UnityEngine;

#if UNITY_STANDALONE
using Steamworks;
#endif

namespace Xsolla.Core
{
	/// <summary>
	/// This ticket you can change for Login JWT
	/// and use it in Xsolla Store
	/// </summary>
	public class SteamSessionTicket
	{
		private string ticket = "";

		private bool Initialized()
		{
			bool result;
			try
			{
				result = SteamManager.Initialized;
			}
			catch (Exception e)
			{
				Debug.LogWarning("Steam initialization error. " + e.Message);
				result = false;
			}

			return result;
		}

		private byte[] GetTicketData()
		{
			byte[] ticket = new byte[1024];
			try
			{
#if UNITY_STANDALONE
				SteamUser.GetAuthSessionTicket(ticket, 1024, out uint length);
				Array.Resize(ref ticket, (int) length);
#else
            ticket = new byte[0];
#endif
			}
			catch (Exception e)
			{
				Debug.Log("Get steam session ticket exception: " + e.Message);
				ticket = new byte[0];
			}

			return ticket;
		}

		private byte[] RefreshTicket()
		{
			return Initialized() ? GetTicketData() : (new byte[0]);
		}

		string ConvertTicket(byte[] ticket)
		{
			StringBuilder sb = new StringBuilder();
			ticket.ToList().ForEach(b => sb.AppendFormat("{0:x2}", b));
			return sb.ToString();
		}

		/// <summary>
		/// Get ticket value for change with Login API
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if (string.IsNullOrEmpty(ticket))
			{
				byte[] ticketData = RefreshTicket();
				ticket = ConvertTicket(ticketData);
				Debug.Log("Requested steam session ticket = " + (string.IsNullOrEmpty(ticket) ? "none" : ticket));
			}

			return ticket;
		}
	}
}
