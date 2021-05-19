using System;
using System.Linq;
using Microsoft.IdentityModel.JsonWebTokens;
using UnityEngine;

namespace Xsolla.Core
{
	public class Token
	{
		private const string EXPIRATION_UNIX_TIME_PARAMETER = "exp";
		private const string USER_ID_URL_PARAMETER = "id";
		private const string CROSS_AUTH_PARAMETER = "is_cross_auth";
		private const string IS_MASTER_ACCOUNT_PARAMETER = "is_master";
		private const string TOKEN_TYPE_PARAMETER = "type";
		private const string TOKEN_PROVIDER_PARAMETER = "provider";
		
		private const string TOKEN_SERVER_TYPE = "server_custom_id";
		private const string TOKEN_SOCIAL_TYPE = "social";
		
		public bool FromSteam { get; set; }
		/// <summary>
		/// Login JWT. To see all fields of this token, you can parse it by <see cref="https://jwt.io/"/>
		/// </summary>
		private JsonWebToken token;
		private string PaystationToken;

		public Token(string encodedToken = null, bool isPaystationToken = false)
		{
			if (!string.IsNullOrEmpty(encodedToken)) {
				if (isPaystationToken) {
					PaystationToken = encodedToken;
				} else {
					token = new JsonWebToken(encodedToken);
				}
			}
			FromSteam = false;
		}

		public string GetSteamUserID()
		{
			if (JWTisNullOrEmpty()) {
				return string.Empty;
			}
			if (!IsCrossAuth()) {
				return string.Empty;
			}
			string steamUserUrl = token.GetPayloadValue<string>(USER_ID_URL_PARAMETER);
			if (string.IsNullOrEmpty(steamUserUrl)) {
				return string.Empty;
			}
			return steamUserUrl.Split('/').ToList().Last();
		}

		private bool IsCrossAuth()
		{
			return !JWTisNullOrEmpty() && token.GetPayloadValue<bool>(CROSS_AUTH_PARAMETER);
		}

		public bool FromSocialNetwork()
		{
			string tokenType;
			if (token.TryGetPayloadValue<string>(TOKEN_TYPE_PARAMETER, out tokenType))
				return tokenType.Equals(TOKEN_SOCIAL_TYPE);

			Debug.LogAssertion(string.Format("Something went wrong... Token must have 'type' parameter. Your token = {0}", token));
			return false;
		}

		public SocialProvider GetSocialProvider()
		{
			if (!FromSocialNetwork())
				return SocialProvider.None;

			string provider;
			if (token.TryGetPayloadValue<string>(TOKEN_PROVIDER_PARAMETER, out provider))
			{
				return Enum.GetValues(typeof(SocialProvider)).Cast<SocialProvider>().
					ToList().DefaultIfEmpty(SocialProvider.None).
					FirstOrDefault(p => p.GetParameter().Equals(provider));
			}

			return SocialProvider.None;
		}

		public bool IsExpired()
		{
			return SecondsLeft() > 0;
		}

		public int SecondsLeft()
		{
			if (JWTisNullOrEmpty()) {
				return 0;
			}
			var expired = token.GetPayloadValue<int>(EXPIRATION_UNIX_TIME_PARAMETER);
			var now = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
			return Mathf.Max((expired - now), 0);
		}

		private bool JWTisNullOrEmpty()
		{
			return
				(token == null) ||
				string.IsNullOrEmpty(token.EncodedToken) ||
				string.IsNullOrEmpty(token.EncodedHeader) ||
				string.IsNullOrEmpty(token.EncodedPayload) ||
				string.IsNullOrEmpty(token.EncodedSignature);
		}

		private bool PaystationIsNullOrEmpty()
		{
			return string.IsNullOrEmpty(PaystationToken);
		}

		public bool IsNullOrEmpty()
		{
			return JWTisNullOrEmpty() && PaystationIsNullOrEmpty();
		}

		public bool IsMasterAccount()
		{
			if (JWTisNullOrEmpty())
				return false;

			string tokenType;
			if (!token.TryGetPayloadValue<string>(TOKEN_TYPE_PARAMETER, out tokenType))
			{
				Debug.LogAssertion(string.Format("Something went wrong... Token must have 'type' parameter. Your token = {0}", token));
				return true;
			}

			if (!tokenType.Equals(TOKEN_SERVER_TYPE))
				return true;

			bool isMaster;
			return token.TryGetPayloadValue<bool>(IS_MASTER_ACCOUNT_PARAMETER, out isMaster) && isMaster;
		}

		public static implicit operator string(Token token) { return (token != null) ? token.ToString() : string.Empty; }
		public static implicit operator Token(string encodedToken) { return new Token(encodedToken); }

		public override string ToString()
		{
			if (!JWTisNullOrEmpty()) {
				return token.EncodedToken;
			}
			return !PaystationIsNullOrEmpty() ? PaystationToken : string.Empty;
		}
	}
}
