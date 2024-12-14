using UnityEngine;

namespace Xsolla.Core
{
	public static class XsollaToken
	{
		private const string SaveKey = "XsollaSuperToken";

		public static string AccessToken => Instance?.accessToken;

		public static string RefreshToken => Instance?.refreshToken;

		public static bool Exists => Instance != null;

		private static TokenData Instance { get; set; }

		public static void Create(string accessToken)
		{
			Instance = new TokenData {
				accessToken = accessToken
			};

			XDebug.Log($"XsollaToken created (access only)\nAccess token: {accessToken}");
			SaveInstance();
		}

		public static void Create(string accessToken, string refreshToken)
		{
			Instance = new TokenData {
				accessToken = accessToken,
				refreshToken = refreshToken
			};

			XDebug.Log($"XsollaToken created (access and refresh)\nAccess token: {accessToken}\nRefresh token: {refreshToken}");
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
				XDebug.Log($"XsollaToken loaded (access only)\nAccess token: {AccessToken}");
			else
				XDebug.Log($"XsollaToken loaded (access and refresh)\nAccess token: {AccessToken}\nRefresh token: {RefreshToken}");

			return true;
		}

		public static void DeleteSavedInstance()
		{
			Instance = null;
			PlayerPrefs.DeleteKey(SaveKey);
		}
	}
}