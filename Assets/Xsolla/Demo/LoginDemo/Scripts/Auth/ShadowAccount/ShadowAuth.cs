using System;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Login;

public class ShadowAuth : MonoBehaviour, ILoginAuthorization
{
	public Action<string> OnSuccess { get;  set; }
	public Action OnFailed { get; set; }

	private string user = "";
	private string platform = "";

	void Start()
	{
		if (XsollaSettings.IsShadow) {
			GenerateRandomUser();
			RequestToken();
		} else {
			OnFailed();
			Destroy(this, 0.1F);
		}
	}

	private void RequestToken()
	{
		XsollaLogin.Instance.SignInShadowAccount(user, platform, SuccessHandler, FailedHandler);
	}

	private void SuccessHandler(string token)
	{
		OnSuccess?.Invoke(token);
		Destroy(this, 0.1F);
	}

	private void FailedHandler(Error error)
	{
		Debug.Log("Failed request token by shadow account with " +
			"user = `" + user + "` and platform = `" + platform + "`. "
			+ error.ToString());
		OnFailed?.Invoke();
		Destroy(this, 0.1F);
	}

	private void GenerateRandomUser()
	{
		string appendix = GenerateRandomAppendix();
		user = XsollaLogin.Instance.ShadowAccountUserID = "sdk_temp_user_id_" + appendix;
		platform = XsollaLogin.Instance.ShadowAccountPlatform = "sdk_test_platform_" + appendix;
	}

	private string GenerateRandomAppendix()
	{
		int datetime = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
		int r = new System.Random().Next();
		return r.ToString() + "_" + datetime.ToString();
	}
}
