package com.XsollaInc.XsollaInGameStoreUnityAsset.androidProxies;

import java.lang.reflect.*;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;

import com.unity3d.player.UnityPlayer;

import com.xsolla.android.login.XLogin;
import com.xsolla.android.login.social.SocialNetwork;
import com.xsolla.android.login.callback.AuthCallback;
import com.xsolla.android.login.callback.StartSocialCallback;
import com.xsolla.android.login.callback.FinishSocialCallback;

public class AndroidAuthProxy extends Activity
{
    private static SocialNetwork targetSocialNetwork;
    private static AuthCallback authCallback;

    public static void authSocial(Activity currentActivity, Activity proxyActivity, SocialNetwork socialNetwork, AuthCallback callback)
    {
        targetSocialNetwork = socialNetwork;
        authCallback = callback;
        currentActivity.startActivity(new Intent(currentActivity, AndroidAuthProxy.class));
    }

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
                authCallback.onError(throwable, String.format("Error:'%s' Message:'%s'",throwable.toString(), errorMessage));
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
                authCallback.onSuccess();
                finish();
            }

            @Override
            public void onAuthCancelled() {
                authCallback.onError(new Throwable(), "CANCELLED");
                finish();
            }

            @Override
            public void onAuthError(Throwable throwable, String errorMessage) {
                authCallback.onError(throwable, String.format("Error:'%s' Message:'%s'",throwable.toString(), errorMessage));
                finish();
            }
        });
    }
}