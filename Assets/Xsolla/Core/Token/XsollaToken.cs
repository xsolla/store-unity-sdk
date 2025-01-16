using System;
using UnityEngine;

namespace Xsolla.Core
{
	public static class XsollaToken
	{
		private const string SaveKey = "XsollaSuperToken";

		/// <summary>
		/// Access token. Required for most API requests.
		/// </summary>
		public static string AccessToken => Instance?.accessToken;

		/// <summary>
		/// Refresh token. Required to get a new access token.
		///	</summary>
		public static string RefreshToken => Instance?.refreshToken;

		/// <summary>
		/// Access token expiration time. Seconds since the Unix epoch.
		///	</summary>
		public static int ExpirationTime => Instance?.expirationTime ?? 0;

		public static bool Exists => Instance != null;

		private static TokenData Instance { get; set; }

		public static void Create(string accessToken)
		{
			Instance = new TokenData {
				accessToken = accessToken
			};

			XDebug.Log("XsollaToken created (access only)"
				+ $"\nAccess token: {accessToken}");

			SaveInstance();
		}

		public static void Create(string accessToken, string refreshToken)
		{
			Instance = new TokenData {
				accessToken = accessToken,
				refreshToken = refreshToken
			};

			XDebug.Log("XsollaToken created (access and refresh)"
				+ $"\nAccess token: {accessToken}"
				+ $"\nRefresh token: {refreshToken}");

			SaveInstance();
		}

		public static void Create(string accessToken, string refreshToken, int expiresIn)
		{
			Instance = new TokenData {
				accessToken = accessToken,
				refreshToken = refreshToken,
				expirationTime = (int) DateTimeOffset.UtcNow.AddSeconds(expiresIn).ToUnixTimeSeconds()
			};

			XDebug.Log("XsollaToken created (access and refresh and expiration time)"
				+ $"\nAccess token: {accessToken}"
				+ $"\nRefresh token: {refreshToken}"
				+ $"\nExpiration time: {DateTimeOffset.FromUnixTimeSeconds(ExpirationTime).ToLocalTime()}");

			SaveInstance();
		}

		private static void SaveInstance()
		{
			if (Instance == null)
				return;

			var json = ParseUtils.ToJson(Instance);
			PlayerPrefs.SetString(SaveKey, json);
		}

		public static bool TryLoadInstance()
		{
			if (!PlayerPrefs.HasKey(SaveKey))
			{
				XDebug.Log("XsollaToken not found in PlayerPrefs");
				return false;
			}

			var json = PlayerPrefs.GetString(SaveKey);
			var data = ParseUtils.FromJson<TokenData>(json);

			if (data == null || string.IsNullOrEmpty(data.accessToken))
			{
				XDebug.Log("XsollaToken not found in PlayerPrefs");
				return false;
			}

			Instance = data;

			if (string.IsNullOrEmpty(RefreshToken))
			{
				XDebug.Log("XsollaToken loaded (access only)"
					+ $"\nAccess token: {AccessToken}");
			}
			else if (ExpirationTime <= 0)
			{
				XDebug.Log("XsollaToken loaded (access and refresh)"
					+ $"\nAccess token: {AccessToken}"
					+ $"\nRefresh token: {RefreshToken}");
			}
			else
			{
				XDebug.Log("XsollaToken loaded (access and refresh and expiration time)"
					+ $"\nAccess token: {AccessToken}"
					+ $"\nRefresh token: {RefreshToken}"
					+ $"\nExpiration time: {DateTimeOffset.FromUnixTimeSeconds(ExpirationTime).ToLocalTime()}");
			}

			return true;
		}

		public static void DeleteSavedInstance()
		{
			Instance = null;
			PlayerPrefs.DeleteKey(SaveKey);
		}
	}
}
