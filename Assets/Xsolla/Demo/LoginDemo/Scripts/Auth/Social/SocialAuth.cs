using System;
using UnityEngine;

public class SocialAuth : MonoBehaviour, ILoginAuthorization
{
	public Action<string> OnSuccess { get; set; }
	public Action OnFailed { get; set; }

	private void Start()
	{
		var socials = FindObjectOfType<SocialAuthContainer>();
		if (socials == null)
		{
			Debug.LogWarning("Can't find `SocialAuthContainer` script. Social auth disabled.");
			return;
		}
		socials.Enable();
		socials.OnSuccess = token => OnSuccess?.Invoke(token);
		socials.OnFailed = () => OnFailed.Invoke();
	}
}
