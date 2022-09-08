package com.XsollaInc.XsollaInGameStoreUnityAsset.androidProxies;

import java.lang.reflect.*;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;

import com.unity3d.player.UnityPlayer;

import com.xsolla.android.login.XLogin;
import com.xsolla.android.login.callback.FinishSocialCallback;
import com.xsolla.android.login.callback.StartSocialCallback;
import com.xsolla.android.login.social.SocialNetwork;

public class AndroidAuthProxy extends Activity
{
    private static SocialNetwork targetSocialNetwork;

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);

        SocialNetwork socialNetwork = targetSocialNetwork;
        XLogin.startSocialAuth(this, socialNetwork, new StartSocialCallback()
        {
            @Override
            public void onAuthStarted() {}

            @Override
            public void onError(Throwable throwable, String errorMessage) {
                sendMessage(socialNetwork.toString(), "ERROR", String.format("Error:'%s' Message:'%s'",throwable.toString(), errorMessage));
                finish();
            }
        });
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data)
    {
        super.onActivityResult(requestCode, resultCode, data);

        SocialNetwork socialNetwork = targetSocialNetwork;

        XLogin.finishSocialAuth(this, socialNetwork, requestCode, requestCode, data,
        new FinishSocialCallback()
        {
            @Override
            public void onAuthSuccess() {
                sendMessage(socialNetwork.toString(), "SUCCESS", XLogin.getToken());
                finish();
            }

            @Override
            public void onAuthCancelled() {
                sendMessage(socialNetwork.toString(), "CANCELLED", null);
                finish();
            }

            @Override
            public void onAuthError(Throwable throwable, String errorMessage) {
                sendMessage(socialNetwork.toString(), "ERROR", String.format("Error:'%s' Message:'%s'",throwable.toString(), errorMessage));
                finish();
            }
        });
    }

    public static void sendMessage(String socialNetwork, String status, String body)
    {
        try
        {
            Class unityPlayer = Class.forName("com.unity3d.player.UnityPlayer");
            Method method = unityPlayer.getMethod("UnitySendMessage", String.class, String.class, String.class);
            StringBuilder builder = new StringBuilder(socialNetwork).append('#').append(status);
            if (body != null) {
                builder.append('#');
                builder.append(body);
            }

            String unityArgument = builder.toString();
            method.invoke(unityPlayer, "SocialNetworks", "ReceiveSocialAuthResult", unityArgument);
        }
        catch (Exception e)
        {
            e.printStackTrace();
        }
    }

    public static void authSocial(Activity currentActivity, Activity proxyActivity, SocialNetwork socialNetwork)
    {
        targetSocialNetwork = socialNetwork;
        currentActivity.startActivity(new Intent(currentActivity, AndroidAuthProxy.class));
    }
}