using System;
using UnityEngine;

public class OldSocialAuth : MonoBehaviour, OldILoginAuthorization
{
	public Action<string> OnSuccess { get; set; }
	public Action OnFailed { get; set; }

#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID
	private void Start()
	{
		var socials = FindObjectOfType<OldSocialAuthContainer>();
		if (socials == null)
		{
			Debug.LogWarning("Can't find `SocialAuthContainer` script. Social auth disabled.");
			return;
		}
		socials.Enable();
		socials.OnSuccess = token => OnSuccess?.Invoke(token);
		socials.OnFailed = () => OnFailed?.Invoke();
	}
#endif
}
