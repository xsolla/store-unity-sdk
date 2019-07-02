using System;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Login
{
	public class XsollaToken
	{
		public string Token
		{
			get
			{
				return PlayerPrefs.HasKey(Constants.XsollaLoginToken)
					? PlayerPrefs.GetString(Constants.XsollaLoginToken)
					: string.Empty;
			}
		}

		public string TokenExp
		{
			get
			{
				return PlayerPrefs.HasKey(Constants.XsollaLoginTokenExp)
					? PlayerPrefs.GetString(Constants.XsollaLoginTokenExp)
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