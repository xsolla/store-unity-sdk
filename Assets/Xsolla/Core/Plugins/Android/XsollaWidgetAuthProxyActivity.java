package com.xsolla.sdk.unity.Example.androidProxies;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;

import androidx.annotation.Nullable;

import com.xsolla.android.login.XLogin;
import com.xsolla.android.login.callback.AuthCallback;
import com.xsolla.android.login.callback.FinishXsollaWidgetAuthCallback;
import com.xsolla.android.login.callback.StartXsollaWidgetAuthCallback;

public class XsollaWidgetAuthProxyActivity extends Activity {
    private static AuthCallback authCallback;

    public static void perform(Activity currentActivity, AuthCallback callback) {
        authCallback = callback;
        currentActivity.startActivity(new Intent(currentActivity, XsollaWidgetAuthProxyActivity.class));
    }

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        if (savedInstanceState != null) {
            finish();
            return;
        }

        XLogin.startAuthWithXsollaWidget(this, new StartXsollaWidgetAuthCallback() {
            @Override
            public void onAuthStarted() {
                Log.d("Xsolla", "onAuthStarted");
            }

            @Override
            public void onError(Throwable throwable, String s) {
                Log.d("Xsolla", "onError");
                finish();
            }
        });
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        XLogin.finishAuthWithXsollaWidget(this, requestCode, resultCode, data, new FinishXsollaWidgetAuthCallback() {
            @Override
            public void onAuthSuccess() {
                Log.d("Xsolla", "onAuthSuccess");
                authCallback.onSuccess();
                finish();
            }

            @Override
            public void onAuthCancelled() {
                Log.d("Xsolla", "onAuthCanceled");
                authCallback.onError(new Throwable(), "CANCELLED");
                finish();
            }

            @Override
            public void onAuthError(Throwable throwable, String errorMessage) {
                authCallback.onError(throwable, String.format("Error:'%s' Message:'%s'", throwable.toString(), errorMessage));
                finish();
            }
        });
    }
}