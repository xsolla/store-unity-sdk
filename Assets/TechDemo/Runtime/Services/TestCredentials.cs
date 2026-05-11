using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla.Tests
{
	/// <summary>
	///     Provides test credentials from a local file or environment variables.
	///     Priority:
	///     1. test-credentials.local.json  (project root, local development, gitignored)
	///     2. XSOLLA_TEST_CREDENTIALS env var  (CI: full JSON content as a single variable)
	///     3. Individual env vars              (CI: fallback per-credential)
	///     To get started locally: copy test-credentials.json.example -> test-credentials.local.json
	///     and fill in the values. Never commit that file.
	///     Individual env vars:
	///     XSOLLA_TEST_OLD_ACCESS_TOKEN
	///     XSOLLA_TEST_OLD_REFRESH_TOKEN
	///     XSOLLA_TEST_DEFAULT_USERNAME
	///     XSOLLA_TEST_DEFAULT_PASSWORD
	///     XSOLLA_TEST_SDK_USERNAME
	///     XSOLLA_TEST_SDK_PASSWORD
	/// </summary>
	public static class TestCredentials
	{
		private static CredentialsFile _file;

		public static string OldAccessToken => Resolve("XSOLLA_TEST_OLD_ACCESS_TOKEN", () => File.oldAccessToken);
		public static string OldRefreshToken => Resolve("XSOLLA_TEST_OLD_REFRESH_TOKEN", () => File.oldRefreshToken);
		public static string DefaultUsername => Resolve("XSOLLA_TEST_DEFAULT_USERNAME", () => File.defaultUser?.username);
		public static string DefaultPassword => Resolve("XSOLLA_TEST_DEFAULT_PASSWORD", () => File.defaultUser?.password);
		public static string DefaultEmail => Resolve("XSOLLA_TEST_DEFAULT_EMAIL", () => File.defaultUser?.email); // assuming email is same as username
		public static string SdkUsername => Resolve("XSOLLA_TEST_SDK_USERNAME", () => File.sdkUser?.username);
		public static string SdkPassword => Resolve("XSOLLA_TEST_SDK_PASSWORD", () => File.sdkUser?.password);

		private static string Resolve(string envVar, Func<string> fromFile)
		{
			var value = fromFile();
			if (!string.IsNullOrEmpty(value))
				return value;

			value = Environment.GetEnvironmentVariable(envVar);
			if (!string.IsNullOrEmpty(value))
				return value;

			throw new InvalidOperationException(
				$"Test credential '{envVar}' is not set. " +
				"Add it to Assets/StreamingAssets/test-credentials.local.json or set the XSOLLA_TEST_CREDENTIALS environment variable.");
		}

		private static CredentialsFile File
		{
			get
			{
				if (_file != null)
					return _file;

				// 1. Local file
				var path = Path.Combine(Application.streamingAssetsPath, "test-credentials.local.json");
				var fileContent = ReadStreamingAsset(path);
				if (fileContent != null)
				{
					_file = JsonUtility.FromJson<CredentialsFile>(fileContent);
					return _file;
				}

				// 2. Full JSON from env var (for CI)
				var json = Environment.GetEnvironmentVariable("XSOLLA_TEST_CREDENTIALS");
				if (!string.IsNullOrEmpty(json))
				{
					_file = JsonUtility.FromJson<CredentialsFile>(json);
					return _file;
				}

				_file = new CredentialsFile();
				return _file;
			}
		}

		private static string ReadStreamingAsset(string path)
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			var request = UnityWebRequest.Get(path);
			request.SendWebRequest();
			while (!request.isDone) { }
			if (request.result == UnityWebRequest.Result.Success)
				return request.downloadHandler.text;
			return null;
#else
			if (System.IO.File.Exists(path))
				return System.IO.File.ReadAllText(path);
			return null;
#endif
		}

		[Serializable]
		private class CredentialsFile
		{
			public string oldAccessToken;
			public string oldRefreshToken;
			public UserEntry defaultUser;
			public UserEntry sdkUser;
		}

		[Serializable]
		private class UserEntry
		{
			public string username;
			public string password;
			public string email;
		}
	}
}