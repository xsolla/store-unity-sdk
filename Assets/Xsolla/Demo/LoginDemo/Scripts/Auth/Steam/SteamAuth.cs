using System;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Login;

public class SteamAuth : MonoBehaviour, ILoginAuthorization
{
	public Action<string> OnSuccess { get ; set; }
	public Action OnFailed { get; set; }
    public string AppID = "480";
    private string ticket = "";

	void Start()
    {
#if  UNITY_STANDALONE || UNITY_EDITOR
        ticket = new SteamSessionTicket().ToString();
#endif
        if (!string.IsNullOrEmpty(ticket)) {
            RequestTokenBy(ticket);
        } else {
            OnFailed?.Invoke();
            Destroy(this, 0.1F);
		}
    }

	void RequestTokenBy(string ticket)
	{
        XsollaLogin.Instance.SteamAuth(AppID, ticket, SuccessHandler, FailedHandler);
    }

	void SuccessHandler(string token)
    {
        OnSuccess?.Invoke(token);
        Destroy(this, 0.1F);
    }

	void FailedHandler(Error error)
	{
        Debug.Log("Token request by steam session ticket = `" + ticket + "` failed! " + error.ToString());
        Destroy(this, 0.1F);
    }
}
