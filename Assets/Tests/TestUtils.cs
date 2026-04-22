using System;
using Xsolla.Core;

namespace Xsolla.Tests
{
	public static class TestUtils
	{
		public static bool IsAuthDataValid()
		{
			var accessToken = XsollaToken.AccessToken;
			if (!string.IsNullOrEmpty(accessToken))
				return false;

			var refreshToken = XsollaToken.RefreshToken;
			if (!string.IsNullOrEmpty(refreshToken))
				return false;

			var expirationTime = XsollaToken.ExpirationTime;
			if (expirationTime > 0)
				return false;

			return true;
		}

		public static string GenerateRandomEmail(int count = 6)
		{
			var username = GenerateRandomString(count);
			var domain = GenerateRandomString(count);
			return $"{username}@{domain}.com";
		}

		public static string GenerateRandomString(int count = 6)
		{
			const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";

			var random = new Random();
			var result = new char[count];

			for (var i = 0; i < count; i++)
				result[i] = chars[random.Next(chars.Length)];

			return new string(result);
		}
	}
}