﻿using System;
using System.Linq;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Xsolla.Core
{
	public class Token
	{
		const string EXPIRATION_UNIX_TIME_PARAMETER = "exp";
		const string USER_ID_URL_PARAMETER = "id";
		const string CROSS_AUTH_PARAMETER = "is_cross_auth";
		const string IS_MASTER_ACCOUNT_PARAMETER = "is_master";

		public bool FromSteam { get; set; }
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

		public bool IsExpired()
		{
			if (JWTisNullOrEmpty()) {
				return false;
			}
			var expired = token.GetPayloadValue<int>(EXPIRATION_UNIX_TIME_PARAMETER);
			var now = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
			return expired <= now;
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
			return token.TryGetPayloadValue<bool>(IS_MASTER_ACCOUNT_PARAMETER, out var isMaster) && isMaster;
		}

		public static implicit operator string(Token token) => token.ToString();
		public static implicit operator Token(string encodedToken) => new Token(encodedToken);

		public override string ToString()
		{
			if (!JWTisNullOrEmpty()) {
				return token.EncodedToken;
			}
			return !PaystationIsNullOrEmpty() ? PaystationToken : string.Empty;
		}
	}
}