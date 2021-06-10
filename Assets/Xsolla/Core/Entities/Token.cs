using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Xsolla.Core
{
	public class Token
	{
		private const string TOKEN_SERVER_TYPE = "server_custom_id";
		private const string TOKEN_SOCIAL_TYPE = "social";

		private string encodedToken;
		private string[] tokenParts;
		private TokenPayload tokenPayload;
		private string paystationToken;

		public bool FromSteam { get; set; }

		public Token(string encodedToken = null, bool isPaystationToken = false)
		{
			if (string.IsNullOrEmpty(encodedToken))
			{
				Debug.LogError("Token is null or empty");
				return;
			}

			if (isPaystationToken)
			{
				paystationToken = encodedToken;
				return;
			}
			else
			{
				this.encodedToken = encodedToken;
			}

			var tokenParts = encodedToken.Split('.');
			if (tokenParts.Length <= 1)
			{
				Debug.LogError($"Token must contain header, payload and signature. Your token parts count was '{tokenParts.Length}'. Your token: {encodedToken}");
				return;
			}

			this.tokenParts = tokenParts;

			var encodedPayload = tokenParts[1];
			TokenPayload parsedPayload;
			if (ParseTokenPayload(encodedPayload, out parsedPayload))
			{
				this.tokenPayload = parsedPayload;
			}
			else
			{
				Debug.LogError("Could not parse token payload");
			}
		}

		public static implicit operator string(Token token) { return (token != null) ? token.ToString() : string.Empty; }
		public static implicit operator Token(string encodedToken) { return new Token(encodedToken); }

		public bool IsCrossAuth()
		{
			if (JWTisNullOrEmpty())
				return false;

			return tokenPayload.is_cross_auth;
		}

		public string GetSteamUserID()
		{
			if (!IsCrossAuth())
				return string.Empty;

			string steamUserUrl = tokenPayload.id;
			if (!string.IsNullOrEmpty(steamUserUrl))
				return steamUserUrl.Split('/').ToList().Last();
			else
				return string.Empty;
		}

		public bool FromSocialNetwork()
		{
			if (JWTisNullOrEmpty())
				return false;

			string tokenType = tokenPayload.type;
			if (!string.IsNullOrEmpty(tokenType))
				return tokenType.Equals(TOKEN_SOCIAL_TYPE);
			else
			{
				Debug.LogAssertion($"Something went wrong... Token must have 'type' parameter. Your token = {encodedToken}");
				return false;
			}
		}

		public SocialProvider GetSocialProvider()
		{
			if (!FromSocialNetwork())
				return SocialProvider.None;

			string provider = tokenPayload.provider;
			if (!string.IsNullOrEmpty(provider))
			{
				return Enum.GetValues(typeof(SocialProvider)).Cast<SocialProvider>().
					ToList().DefaultIfEmpty(SocialProvider.None).
					FirstOrDefault(p => p.GetParameter().Equals(provider));
			}
			else
				return SocialProvider.None;
		}

		public int SecondsLeft()
		{
			if (JWTisNullOrEmpty())
				return 0;

			var expired = tokenPayload.exp;
			var now = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
			return Mathf.Max((expired - now), 0);
		}

		public bool IsExpired()
		{
			return SecondsLeft() <= 0;
		}

		public bool IsMasterAccount()
		{
			if (JWTisNullOrEmpty())
				return false;

			string tokenType = tokenPayload.type;
			if (string.IsNullOrEmpty(tokenType))
			{
				Debug.LogAssertion(string.Format("Something went wrong... Token must have 'type' parameter. Your token = {0}", encodedToken));
				return true;
			}

			if (!tokenType.Equals(TOKEN_SERVER_TYPE))
				return true;

			return tokenPayload.is_master;
		}

		public bool IsNullOrEmpty()
		{
			return JWTisNullOrEmpty() && PaystationIsNullOrEmpty();
		}

		public override string ToString()
		{
			if (!JWTisNullOrEmpty())
				return encodedToken;
			else if (!PaystationIsNullOrEmpty())
				return paystationToken;
			else
				return string.Empty;
		}

		private bool JWTisNullOrEmpty()
		{
			if (encodedToken == null)
				return true;

			if (tokenParts == null || tokenParts.Length != 3)
				return true;

			if (tokenPayload == null)
				return true;

			//else
			return false;
		}

		private bool PaystationIsNullOrEmpty()
		{
			return string.IsNullOrEmpty(paystationToken);
		}

		private bool ParseTokenPayload(string encodedPayload, out TokenPayload payloadJsonObject)
		{
			var padding = encodedPayload.Length % 4;
			if (padding != 0)
			{
				var paddingToAdd = 4 - padding;
				encodedPayload = encodedPayload + new string('=', paddingToAdd);
			}

			try
			{
				var bytes = Convert.FromBase64String(encodedPayload);
				var decodedPayload = Encoding.UTF8.GetString(bytes);
				payloadJsonObject = ParseUtils.FromJson<TokenPayload>(decodedPayload);
			}
			catch (Exception ex)
			{
				Debug.LogError($"Error decoding token payload: {ex.Message}");
				payloadJsonObject = null;
				return false;
			}

			return true;
		}

		[Serializable]
		public class TokenPayload
		{
			public int exp;
			public string id;
			public bool is_cross_auth;
			public bool is_master;
			public string type;
			public string provider;
		}
	}
}
