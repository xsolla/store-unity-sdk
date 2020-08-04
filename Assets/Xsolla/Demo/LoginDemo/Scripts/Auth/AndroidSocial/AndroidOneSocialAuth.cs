using System;
using UnityEngine;
using UnityEngine.UI;

public class AndroidOneSocialAuth : MonoBehaviour, ISocialAuthorization
{
	public Action<string> OnSuccess { get; set; }
	public Action OnFailed { get; set; }

	[SerializeField]
	private SocialProvider provider;
	
	private Button _authButton;
    private AndroidSDKSocialAuthListener _listener;

	private void Start()
	{
#if UNITY_ANDROID
		_authButton = GetComponent<Button>();
#else
		gameObject.SetActive(false);
#endif
	}

	public void Enable()
	{
#if UNITY_ANDROID
		if (_authButton != null)
		{
			_authButton.onClick.AddListener(SocialNetworkAuth);
		}
	}
	
	private void SocialNetworkAuth()
	{
        SetListener();

        try
        {
            using (var sdkHelper = new AndroidSDKSocialAuthHelper())
            {
                sdkHelper.PerformSocialAuth(provider);
            }

            Debug.Log("AndroidOneSocialAuth.SocialNetworkAuth: auth request was sent");
        }
        catch (Exception ex)
        {
            Debug.LogError($"AndroidOneSocialAuth.SocialNetworkAuth: {ex.Message}");
            RemoveListener();
            OnFailed?.Invoke();
        }
    }

    private void SetListener()
    {
        _listener = this.transform.parent.gameObject.AddComponent<AndroidSDKSocialAuthListener>();
        _listener.OnSocialAuthResult += OnSocialAuthResult;
    }

    private void RemoveListener()
    {
        _listener.OnSocialAuthResult -= OnSocialAuthResult;
        Destroy(_listener);
    }

    private void OnSocialAuthResult(string authResult)
    {
        RemoveListener();

        if (authResult == null)
        {
            Debug.LogError("AndroidOneSocialAuth.OnSocialAuthResult: authResult == null");
            return;
        }

        var args = authResult.Split('#');

        if (args.Length != 3)
        {
            Debug.LogError($"AndroidOneSocialAuth.OnSocialAuthResult: args.Length != 3. Result was {authResult}");
            return;
        }

        var socialProvider = args[0];

        if (socialProvider.ToUpper() != this.provider.ToString().ToUpper())
        {
            Debug.LogError($"AndroidOneSocialAuth.OnSocialAuthResult: args.Provider was {socialProvider} when expected {this.provider}");
            return;
        }

        Debug.Log($"AndroidOneSocialAuth.OnSocialAuthResult: processing auth result for {socialProvider}");

        var authStatus = args[1].ToUpper();
        var messageBody = args[2];
        var logHeader = $"AndroidOneSocialAuth.OnSocialAuthResult: authResult for {socialProvider} returned";

        switch (authStatus)
        {
            case "SUCCESS":
                Debug.Log($"{logHeader} SUCCESS. Token: {messageBody}");
                OnSuccess?.Invoke(messageBody);
                break;
            case "ERROR":
                Debug.LogError($"{logHeader} ERROR. Error message: {messageBody}");
                OnFailed?.Invoke();
                break;
            case "CANCELLED":
                Debug.Log($"{logHeader} CANCELLED. Additional info: {messageBody}");
                break;
            default:
                Debug.LogError($"{logHeader} unexpected authResult: {authStatus}");
                break;
        }
#endif
	}
}
