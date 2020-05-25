using System;
using System.Linq;
using UnityEngine;

public class SocialAuthContainer : MonoBehaviour, ILoginAuthorization
{
#if UNITY_STANDALONE || UNITY_EDITOR
    public void Enable()
    {
        transform.GetComponentsInChildren<OneSocialAuth>().ToList().ForEach(auth =>
            {
                auth.OnSuccess = token =>  OnSuccess.Invoke(token);
                auth.OnFailed = () => OnFailed.Invoke();
                auth.Enable();
            });
    }
#endif
    
    public Action<string> OnSuccess { get; set; }
    public Action OnFailed { get; set; }
}
