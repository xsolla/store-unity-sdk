using System;
using UnityEngine;

public class SocialAuthContainer : MonoBehaviour, ILoginAuthorization
{
#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID
    public void Enable()
    {
        var auths = transform.GetComponentsInChildren<ISocialAuthorization>();

        foreach (var auth in auths)
        {
            auth.OnSuccess = token =>  OnSuccess.Invoke(token);
            auth.OnFailed = () => OnFailed.Invoke();
            auth.Enable();
        }
    }
#endif

    public Action<string> OnSuccess { get; set; }
    public Action OnFailed { get; set; }
}
