using System;
using UnityEngine;
using Xsolla.Core;


public class AndroidSDKSocialAuthHelper : IDisposable
{
    private AndroidHelper _androidHelper;

    public AndroidSDKSocialAuthHelper()
    {
        GetXsollaSettings(out string loginID, out string callbackURL, out string facebookAppId, out string googleServerId);
        _androidHelper = new AndroidHelper();

        try
        {
            var xlogin = new AndroidJavaClass("com.xsolla.android.login.XLogin");
            var context = _androidHelper.ApplicationContext;
            var socialConfigBuilder = new AndroidJavaObject("com.xsolla.android.login.XLogin$SocialConfig$Builder");
            
            socialConfigBuilder.Call<AndroidJavaObject>("facebookAppId", facebookAppId);
            socialConfigBuilder.Call<AndroidJavaObject>("googleServerId", googleServerId);

            var socialConfig = socialConfigBuilder.Call<AndroidJavaObject>("build");

            if (!string.IsNullOrEmpty(callbackURL))
                xlogin.CallStatic("init", loginID, callbackURL, context, socialConfig);
            else
                xlogin.CallStatic("init", loginID, context, socialConfig);
        }
        catch (Exception ex)
        {
            throw new AggregateException($"AndroidSDKSocialAuthHelper.Ctor: {ex.Message}", ex); 
        }
    }

    public void PerformSocialAuth(SocialProvider socialProvider)
    {
        var providerName = socialProvider.ToString().ToUpper();

        try
        {
            var unitySDKHelper = new AndroidJavaClass("com.xsolla.android.login.XLogin$Unity");
            var actvity = _androidHelper.CurrentActivity;
            var socialNetworkClass = new AndroidJavaClass("com.xsolla.android.login.social.SocialNetwork");
            var socialNetworkObject = socialNetworkClass.GetStatic<AndroidJavaObject>(providerName);

            unitySDKHelper.CallStatic("authSocial", actvity, socialNetworkObject);
        }
        catch (Exception ex)
        {
            throw new AggregateException($"AndroidSDKSocialAuthHelper.PerformSocialAuth: {ex.Message}", ex);
        }
    }

    private void GetXsollaSettings(out string loginID, out string callbackURL, out string facebookAppId, out string googleServerId)
    {
        loginID = XsollaSettings.LoginId;
        callbackURL = XsollaSettings.CallbackUrl;
        facebookAppId = XsollaSettings.FacebookAppId;
        googleServerId = XsollaSettings.GoogleServerId;
    }

    public void Dispose()
    {
        _androidHelper.Dispose();
    }
}
