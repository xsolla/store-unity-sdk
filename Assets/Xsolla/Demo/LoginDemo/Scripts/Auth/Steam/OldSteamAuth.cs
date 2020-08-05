using System;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Login;

public class OldSteamAuth : MonoBehaviour, OldILoginAuthorization
{
	public Action<string> OnSuccess { get ; set; }
	public Action OnFailed { get; set; }
	private string _ticket = "";

	void Start()
    {
	    if (!XsollaSettings.UseSteamAuth)
	    {
		    OnFailed?.Invoke();
		    Destroy(this, 0.1F);
		    return;
	    }
#if  UNITY_STANDALONE || UNITY_EDITOR
        _ticket = new SteamSessionTicket().ToString();
#endif
        if (!string.IsNullOrEmpty(_ticket)) {
            RequestTokenBy(_ticket);
        } else {
            OnFailed?.Invoke();
            Destroy(this, 0.1F);
		}
    }

	void RequestTokenBy(string ticket)
	{
		if (int.TryParse(XsollaSettings.SteamAppId, out _))
		{
			XsollaLogin.Instance.SteamAuth(XsollaSettings.SteamAppId, ticket, SuccessHandler, FailedHandler);			
		}
		else
		{
			Debug.LogWarning("Can't parse SteamAppId = " + XsollaSettings.SteamAppId);
			OnFailed?.Invoke();
		}
	}

	void SuccessHandler(string token)
    {
        OnSuccess?.Invoke(token);
        Destroy(this, 0.1F);
    }

	void FailedHandler(Error error)
	{
        Debug.Log("Token request by steam session ticket = `" + _ticket + "` failed! " + error.ToString());
        OnFailed?.Invoke();
        Destroy(this, 0.1F);
    }
}
