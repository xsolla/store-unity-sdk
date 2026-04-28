using System;
using System.IO;
using UnityEngine;

namespace Xsolla.Tests
{
	/// <summary>
	/// Provides test credentials from a local file or environment variables.
	///
	/// Priority:
	///   1. test-credentials.local.json  (project root, local development, gitignored)
	///   2. XSOLLA_TEST_CREDENTIALS env var  (CI: full JSON content as a single variable)
	///   3. Individual env vars              (CI: fallback per-credential)
	///
	/// To get started locally: copy test-credentials.json.example -> test-credentials.local.json
	/// and fill in the values. Never commit that file.
	///
	/// Individual env vars:
	///   XSOLLA_TEST_OLD_ACCESS_TOKEN
	///   XSOLLA_TEST_OLD_REFRESH_TOKEN
	///   XSOLLA_TEST_DEFAULT_USERNAME
	///   XSOLLA_TEST_DEFAULT_PASSWORD
	///   XSOLLA_TEST_SDK_USERNAME
	///   XSOLLA_TEST_SDK_PASSWORD
	/// </summary>
	internal static class TestCredentials
	{
		private static CredentialsFile _file;

		public static string OldAccessToken => Resolve("XSOLLA_TEST_OLD_ACCESS_TOKEN", () => File.oldAccessToken);
		public static string OldRefreshToken => Resolve("XSOLLA_TEST_OLD_REFRESH_TOKEN", () => File.oldRefreshToken);
		public static string DefaultUsername => Resolve("XSOLLA_TEST_DEFAULT_USERNAME", () => File.defaultUser?.username);
		public static string DefaultPassword => Resolve("XSOLLA_TEST_DEFAULT_PASSWORD", () => File.defaultUser?.password);
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
				$"Add it to Assets/Tests/test-credentials.local.json or set the XSOLLA_TEST_CREDENTIALS environment variable.");
		}

		private static CredentialsFile File
		{
			get
			{
				if (_file != null)
					return _file;

				// 1. Local file
				var path = Path.Combine(Application.dataPath, "..", "test-credentials.local.json");
				if (System.IO.File.Exists(path))
				{
					_file = JsonUtility.FromJson<CredentialsFile>(System.IO.File.ReadAllText(path));
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
		}
	}
}
