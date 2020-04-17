/// <summary>
/// Social providers list for Login Social Auth.
/// See full list at <see cref="https://developers.xsolla.com/login-api/jwt/jwt-auth-via-social-network/"/>.
/// </summary>
public enum SocialProvider
{
    Apple,
    BattleNet,
    Discord,
    Facebook,
    GitHub,
    Google,
    GooglePlus,
    Twitch,
    Twitter,
    VK,
    XBox
}

public static class SocialProviderConverter
{
    public static string GetParameter(this SocialProvider provider)
    {
        switch (provider)
        {
            case SocialProvider.Apple: return "apple";
            case SocialProvider.BattleNet: return "battlenet";
            case SocialProvider.Discord: return "discord";
            case SocialProvider.Facebook: return "facebook";
            case SocialProvider.GitHub: return "github";
            case SocialProvider.Google: return "google";
            case SocialProvider.GooglePlus: return "google+";
            case SocialProvider.Twitch: return "twitch";
            case SocialProvider.Twitter: return "twitter";
            case SocialProvider.VK: return "vk";
            case SocialProvider.XBox: return "xbox";
            default: return string.Empty;
        }
    }
}