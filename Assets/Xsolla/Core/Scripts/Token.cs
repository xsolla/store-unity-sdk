using System;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Xsolla.Core
{
	public class Token : JsonWebToken
	{
		const string EXPIRATION_UNIX_TIME_PARAMETER = "exp";

		public Token(string encodedToken) : base(encodedToken)
		{}

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
