using System;

namespace Xsolla.Core
{
	/// <summary>
	/// Social providers list for Login Social Auth.
	/// See full list at <see cref="https://developers.xsolla.com/login-api/jwt/jwt-auth-via-social-network/"/>.
	/// </summary>
	public enum SocialProvider
	{
		None,
		Facebook,
		GitHub,
		Google,
		Twitch,
		Twitter,
		Steam,
		Xbox,
		Discord,
		Vk,
		Naver,
		Kakao,
		Baidu,
		Battlenet,
		ChinaTelecom,
		Instagram,
		LinkedIn,
		Odnoklassniki,
		PayPal,
		Pinterest,
		QQ,
		Reddit,
		Vimeo,
		WeChat,
		Weibo,
		Yahoo,
		Amazon,
		Mailru,
		Microsoft,
		Msn,
		Yandex,
		YouTube,
		Apple
	}

	public static class SocialProviderConverter
	{
		public static string GetParameter(this SocialProvider provider)
		{
			switch (provider)
			{
				case SocialProvider.Facebook:      return "facebook";
				case SocialProvider.GitHub:        return "github";
				case SocialProvider.Google:        return "google";
				case SocialProvider.Twitch:        return "twitch";
				case SocialProvider.Twitter:       return "twitter";
				case SocialProvider.Steam:         return "steam";
				case SocialProvider.Xbox:          return "xbox";
				case SocialProvider.Discord:       return "discord";
				case SocialProvider.Vk:            return "vk";
				case SocialProvider.Naver:         return "naver";
				case SocialProvider.Kakao:         return "kakao";
				case SocialProvider.Baidu:         return "baidu";
				case SocialProvider.Battlenet:     return "battlenet";
				case SocialProvider.ChinaTelecom:  return "chinatelecom";
				case SocialProvider.Instagram:     return "instagram";
				case SocialProvider.LinkedIn:      return "linkedin";
				case SocialProvider.Odnoklassniki: return "ok";
				case SocialProvider.PayPal:        return "paypal";
				case SocialProvider.Pinterest:     return "pinterest";
				case SocialProvider.QQ:            return "qq";
				case SocialProvider.Reddit:        return "reddit";
				case SocialProvider.Vimeo:         return "vimeo";
				case SocialProvider.WeChat:        return "wechat";
				case SocialProvider.Weibo:         return "weibo";
				case SocialProvider.Yahoo:         return "yahoo";
				case SocialProvider.Amazon:        return "amazon";
				case SocialProvider.Mailru:        return "mailru";
				case SocialProvider.Microsoft:     return "microsoft";
				case SocialProvider.Msn:           return "msn";
				case SocialProvider.Yandex:        return "yandex";
				case SocialProvider.YouTube:       return "youtube";
				case SocialProvider.Apple:         return "apple";
				default:                           throw new Exception(provider.ToString());
			}
		}
	}

	[Serializable]
	public class LinkedSocialNetwork
	{
		public string full_name;
		public string nickname;
		public string picture;
		public string provider;
		public string social_id;
	}
}