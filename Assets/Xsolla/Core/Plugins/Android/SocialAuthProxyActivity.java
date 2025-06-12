package com.xsolla.sdk.unity.Example.androidProxies;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;

import com.xsolla.android.login.XLogin;
import com.xsolla.android.login.social.SocialNetwork;
import com.xsolla.android.login.callback.AuthCallback;
import com.xsolla.android.login.callback.StartSocialCallback;
import com.xsolla.android.login.callback.FinishSocialCallback;

public class SocialAuthProxyActivity extends Activity
{
    private static SocialNetwork targetSocialNetwork;
    private static AuthCallback authCallback;
    private static final String TAG = "SocialAuthProxyActivity";

    public static void perform(Activity currentActivity, SocialNetwork socialNetwork, AuthCallback callback)
    {
        targetSocialNetwork = socialNetwork;
        authCallback = callback;
        currentActivity.startActivity(new Intent(currentActivity, SocialAuthProxyActivity.class));
    }

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);

        SocialNetwork socialNetwork = targetSocialNetwork;
        if (socialNetwork == null) {
            Log.e(TAG, "Target social network is null. Exiting.");
            finish();
            return;
        }
        
        XLogin.startSocialAuth(this, socialNetwork, new StartSocialCallback()
        {
            @Override
            public void onAuthStarted() {
                // No action needed here, as the auth process is started
            }

            @Override
            public void onError(Throwable throwable, String errorMessage) {
                if (authCallback != null) {
                    Throwable safeThrowable = throwable != null ? throwable : new Throwable("Unknown error");
                    String errorStr = throwable != null ? throwable.toString() : "null";
                    String messageStr = errorMessage != null ? errorMessage : "null";
                    authCallback.onError(safeThrowable, String.format("Error:'%s' Message:'%s'", errorStr, messageStr));
                    Log.e(TAG, "Error during social auth: " + errorStr + " Message: " + messageStr);
                }
                finish();
            }
        });
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data)
    {
        super.onActivityResult(requestCode, resultCode, data);

        SocialNetwork socialNetwork = targetSocialNetwork;
        if (socialNetwork == null) {
            Log.e(TAG, "Target social network is null. Exiting.");
            finish();
            return;
        }

        XLogin.finishSocialAuth(this, socialNetwork, requestCode, requestCode, data,
        new FinishSocialCallback()
        {
            @Override
            public void onAuthSuccess() {
                if (authCallback != null) {
                    authCallback.onSuccess();
                }
                finish();
            }

            @Override
            public void onAuthCancelled() {
                if (authCallback != null) {
                    authCallback.onError(new Throwable(), "CANCELLED");
                }
                finish();
            }

            @Override
            public void onAuthError(Throwable throwable, String errorMessage) {
                if (authCallback != null) {
                    Throwable safeThrowable = throwable != null ? throwable : new Throwable("Unknown error");
                    String errorStr = throwable != null ? throwable.toString() : "null";
                    String messageStr = errorMessage != null ? errorMessage : "null";
                    authCallback.onError(safeThrowable, String.format("Error:'%s' Message:'%s'", errorStr, messageStr));
                    Log.e(TAG, "Error during social auth: " + errorStr + " Message: " + messageStr);
                }
                finish();
            }
        });
    }
}