package com.xsolla.sdk.unity.Example.androidProxies;

public interface AdditionalInfoAuthCallback {
    void onSuccess(String code, String token);
    void onCancel();
    void onError(String message);
}
