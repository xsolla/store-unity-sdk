using System;
using UnityEngine;

/// <summary>
/// Class that handle token from XsollaLauncher product.
/// <see cref="https://developers.xsolla.com/doc/launcher/"/> for more information.
/// </summary>
public class OldLauncherAuth : MonoBehaviour, OldILoginAuthorization
{
	public Action<string> OnSuccess { get; set; }
	public Action OnFailed { get; set; }

	private void Start()
	{
		string launcherToken = LauncherArguments.Instance.GetToken();
		if (!string.IsNullOrEmpty(launcherToken)) {
			OnSuccess?.Invoke(launcherToken);
		} else {
			OnFailed?.Invoke();
		}
		Destroy(this, 0.1F);
	}
}
