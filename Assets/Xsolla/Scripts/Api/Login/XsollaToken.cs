using System;
using UnityEngine;

namespace Xsolla.Login
{
	public class XsollaToken
	{
		public string Token
		{
			get
			{
				return PlayerPrefs.HasKey(XsollaConstants.Prefs_Token)
					? PlayerPrefs.GetString(XsollaConstants.Prefs_Token)
					: string.Empty;
			}
		}

		public string TokenExp
		{
			get
			{
				return PlayerPrefs.HasKey(XsollaConstants.Prefs_TokenExp)
					? PlayerPrefs.GetString(XsollaConstants.Prefs_TokenExp)
					: string.Empty;
			}
		}

		public bool IsTokenValid
		{
			get
			{
				long epochTicks = new DateTime(1970, 1, 1).Ticks;
				long unixTime = ((DateTime.UtcNow.Ticks - epochTicks) / TimeSpan.TicksPerSecond);

				if (!string.IsNullOrEmpty(TokenExp))
					return long.Parse(TokenExp) >= unixTime;
				else
					return false;
			}
		}
	}
}