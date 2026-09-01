package com.xsolla.sdk.unity.Example.androidProxies;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.text.TextUtils;
import android.util.Log;
import android.webkit.WebResourceRequest;
import android.webkit.WebView;
import android.webkit.WebViewClient;

import java.util.Locale;

public class AdditionalInfoAuthProxyActivity extends Activity {
    private static final String TAG = "AdditionalInfoAuthProxyActivity";
    private static final String ARG_LOGIN_URL = "login_url";
    private static final String ARG_REDIRECT_URL = "redirect_url";
    private static final String ASK_URL_PREFIX = "https://login-widget.xsolla.com/latest/ask";

    private static AdditionalInfoAuthCallback authCallback;

    public static void perform(Activity currentActivity, AdditionalInfoAuthCallback callback, String loginUrl, String redirectUrl) {
        authCallback = callback;

        Intent intent = new Intent(currentActivity, AdditionalInfoAuthProxyActivity.class);
        intent.putExtra(ARG_LOGIN_URL, loginUrl);
        intent.putExtra(ARG_REDIRECT_URL, redirectUrl);
        currentActivity.startActivity(intent);
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        if (savedInstanceState != null) {
            emitCancelAndFinish();
            return;
        }

        Intent intent = getIntent();
        if (intent == null) {
            emitErrorAndFinish("Additional info auth intent is null");
            return;
        }

        String loginUrl = intent.getStringExtra(ARG_LOGIN_URL);
        String redirectUrl = intent.getStringExtra(ARG_REDIRECT_URL);
        if (TextUtils.isEmpty(loginUrl)) {
            emitErrorAndFinish("Additional info auth login URL is empty");
            return;
        }

        setupWebView(loginUrl, redirectUrl);
    }

    @SuppressLint("SetJavaScriptEnabled")
    private void setupWebView(String loginUrl, String redirectUrl) {
        WebView webView = new WebView(this);
        setContentView(webView);

        webView.getSettings().setJavaScriptEnabled(true);
        webView.getSettings().setDomStorageEnabled(true);
        webView.getSettings().setSupportMultipleWindows(false);
        webView.getSettings().setJavaScriptCanOpenWindowsAutomatically(true);

        webView.setWebViewClient(new WebViewClient() {
            @Override
            public boolean shouldOverrideUrlLoading(WebView view, String url) {
                return handleUrl(url, redirectUrl);
            }

            @Override
            public boolean shouldOverrideUrlLoading(WebView view, WebResourceRequest request) {
                Uri uri = request != null ? request.getUrl() : null;
                return handleUrl(uri != null ? uri.toString() : null, redirectUrl);
            }
        });

        webView.loadUrl(loginUrl);
    }

    private boolean handleUrl(String url, String redirectUrl) {
        if (TextUtils.isEmpty(url))
            return false;

        if (!shouldHandleUrl(url, redirectUrl))
            return false;

        try {
            Uri uri = Uri.parse(url);
            String code = uri.getQueryParameter("code");
            String token = uri.getQueryParameter("token");
            String errorDescription = uri.getQueryParameter("error_description");
            String error = uri.getQueryParameter("error");

            if (!TextUtils.isEmpty(token) || !TextUtils.isEmpty(code)) {
                emitSuccessAndFinish(code, token);
                return true;
            }

            if (!TextUtils.isEmpty(errorDescription) || !TextUtils.isEmpty(error)) {
                emitErrorAndFinish(!TextUtils.isEmpty(errorDescription) ? errorDescription : error);
                return true;
            }

            emitErrorAndFinish("Additional info auth URL does not contain code or token");
            return true;
        } catch (Exception exception) {
            emitErrorAndFinish("Failed to parse additional info auth URL: " + exception.getMessage());
            return true;
        }
    }

    private boolean shouldHandleUrl(String url, String redirectUrl) {
        String normalized = url.toLowerCase(Locale.ROOT);
        if (normalized.startsWith(ASK_URL_PREFIX))
            return true;

        if (!TextUtils.isEmpty(redirectUrl))
            return normalized.startsWith(redirectUrl.toLowerCase(Locale.ROOT));

        return false;
    }

    private void emitSuccessAndFinish(String code, String token) {
        Log.d(TAG, "Additional info auth completed");
        if (authCallback != null)
            authCallback.onSuccess(code, token);
        finish();
    }

    private void emitErrorAndFinish(String message) {
        String safeMessage = TextUtils.isEmpty(message)
                ? "Unknown additional info auth error"
                : message;
        Log.e(TAG, safeMessage);
        if (authCallback != null)
            authCallback.onError(safeMessage);
        finish();
    }

    private void emitCancelAndFinish() {
        Log.d(TAG, "Additional info auth cancelled");
        if (authCallback != null)
            authCallback.onCancel();
        finish();
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        if (isFinishing())
            authCallback = null;
    }

    @Override
    public void onBackPressed() {
        emitCancelAndFinish();
    }
}
