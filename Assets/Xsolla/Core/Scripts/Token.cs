using System;
using System.Linq;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Xsolla.Core
{
	public class Token : JsonWebToken
	{
		const string EXPIRATION_UNIX_TIME_PARAMETER = "exp";
		const string USER_ID_URL_PARAMETER = "id";
		const string CROSS_AUTH_PARAMETER = "is_cross_auth";

		public bool FromSteam { get; set; }

		public Token(string encodedToken) : base(encodedToken)
		{
			FromSteam = false;
		}

		public string GetSteamUserID()
		{
			if (!IsCrossAuth()) {
				return string.Empty;
			}
			string steamUserUrl = GetPayloadValue<string>(USER_ID_URL_PARAMETER);
			if (string.IsNullOrEmpty(steamUserUrl)) {
				return string.Empty;
			}
			return steamUserUrl.Split('/').ToList().Last();
		}

		private bool IsCrossAuth()
		{
			return GetPayloadValue<bool>(CROSS_AUTH_PARAMETER);
		}

		public bool IsExpired()
		{
			int expired = GetPayloadValue<int>(EXPIRATION_UNIX_TIME_PARAMETER);
			int now = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
			return expired <= now;
		}

		public bool IsNullOrEmpty()
		{
			return string.IsNullOrEmpty(EncodedToken) ||
				string.IsNullOrEmpty(EncodedHeader) ||
				string.IsNullOrEmpty(EncodedPayload) ||
				string.IsNullOrEmpty(EncodedSignature);
		}

		public static implicit operator string(Token token) => token.EncodedToken;
		public static implicit operator Token(string encodedToken) => new Token(encodedToken);

		public override string ToString()
		{
			return EncodedToken;
		}
	}
}
