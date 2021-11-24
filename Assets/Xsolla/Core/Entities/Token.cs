using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Xsolla.Core
{
	public class Token
	{
		private string EncodedToken { get; set; }

		private TokenPayload Payload { get; set; }

		private bool IsPaystationToken
		{
			get
			{
				return (Payload == null);
			}
		}

		private const string PlayerPrefsKey = "xsolla_login_last_success_auth_token";

		public bool IsMasterAccount()
		{
			if (IsPaystationToken)
				return false;

			return Payload.is_master;
		}

		public int SecondsLeft()
		{
			if (IsPaystationToken)
				return 0;

			var now = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
			return Mathf.Max(Payload.exp - now, 0);
		}

		public bool FromSocialNetwork()
		{
			if (IsPaystationToken)
				return false;

			return Payload.type == "social";
		}

		public SocialProvider GetSocialProvider()
		{
			if (!FromSocialNetwork())
				return SocialProvider.None;

			var provider = Payload.provider;
			if (string.IsNullOrEmpty(provider))
				return SocialProvider.None;

			return (SocialProvider)Enum.Parse(typeof(SocialProvider), provider);
		}

		public string GetSteamUserID()
		{
			if (IsPaystationToken)
				return string.Empty;

			if (!Payload.is_cross_auth)
				return string.Empty;

			var steamUserUrl = Payload.id;
			if (string.IsNullOrEmpty(steamUserUrl))
				return string.Empty;

			return steamUserUrl.Split('/').Last();
		}

		private static Token _instance;

		public static Token Instance
		{
			get
			{
				return _instance;
			}
			set
			{
				_instance = value;
				if (TokenChanged != null)
					TokenChanged.Invoke();
			}
		}

		public static event Action TokenChanged;

		public static Token Create(string encodedToken)
		{
			if (string.IsNullOrEmpty(encodedToken))
				throw new Exception("Encoded token argument is null or empty");

			var tokenPartsCount = encodedToken.Split('.').Length;
			return tokenPartsCount == 3
				? CreateJwtToken(encodedToken)
				: CreatePaystationToken(encodedToken);
		}

		public static void Save()
		{
			if (Instance == null)
				return;

			PlayerPrefs.SetString(PlayerPrefsKey, Instance.EncodedToken);
		}

		public static bool Load()
		{
			if (!PlayerPrefs.HasKey(PlayerPrefsKey))
				return false;

			var encodedToken = PlayerPrefs.GetString(PlayerPrefsKey);
			if (string.IsNullOrEmpty(encodedToken))
				return false;

			Instance = Create(encodedToken);
			return true;
		}

		public static void DeleteSave()
		{
			PlayerPrefs.DeleteKey(PlayerPrefsKey);
		}

		private static Token CreateJwtToken(string encodedToken)
		{
			if (string.IsNullOrEmpty(encodedToken))
				throw new Exception("Encoded token argument is null or empty");

			var tokenParts = encodedToken.Split('.');
			if (tokenParts.Length < 3)
				throw new Exception(string.Format("Token must contain header, payload and signature. Your token parts count was '{0}'. Your token: {1}", tokenParts.Length, encodedToken));

			TokenPayload payload;
			if (!TryParsePayload(tokenParts[1], out payload))
				throw new Exception(string.Format("Could not parse token payload. Your token = {0}", encodedToken));

			if (string.IsNullOrEmpty(payload.type))
				throw new Exception(string.Format("Token must have 'type' parameter. Your token = {0}", encodedToken));

			return new Token{
				EncodedToken = encodedToken,
				Payload = payload
			};
		}

		private static Token CreatePaystationToken(string encodedToken)
		{
			return new Token{
				EncodedToken = encodedToken
			};
		}

		private Token()
		{
		}

		[Obsolete()]
		private Token(string encodedToken = null, bool isPaystationToken = false)
		{
			Instance = Create(encodedToken);
			EncodedToken = Instance.EncodedToken;
			Payload = Instance.Payload;
		}

		public override string ToString()
		{
			return EncodedToken;
		}

		public static implicit operator string(Token token)
		{
			return token != null ? token.EncodedToken : string.Empty;
		}

		private static bool TryParsePayload(string encodedPayload, out TokenPayload payloadObject)
		{
			//Fixed FromBase64String convertion.
			encodedPayload = encodedPayload.Replace('-', '+').Replace('_', '/');

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
				payloadObject = ParseUtils.FromJson<TokenPayload>(decodedPayload);
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Error decoding token payload: {0}", ex.Message));
				payloadObject = null;
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
