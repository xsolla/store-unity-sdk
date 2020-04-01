using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Xsolla.Core;

public class BasicAuthButton : MonoBehaviour
{
	private Button button;
	private Func<bool> isEnabled;

	private DateTime lastClick;
	private readonly float rateLimitMs = Constants.LoginPageRateLimitMs;

	private void Awake()
	{
		lastClick = DateTime.MinValue;
	}

	private void Update()
	{
		if(isEnabled != null) {
			button.interactable = isEnabled.Invoke();
		}
	}

	public BasicAuthButton SetButton(Button button)
	{
		this.button = button;
		this.button.interactable = false;
		return this;
	}

	public BasicAuthButton SetActiveCondition(Func<bool> condition)
	{
		isEnabled = condition;
		return this;
	}

	public BasicAuthButton SetHandler(UnityAction clickHandler)
	{
		button.onClick.AddListener(
			WithRateLimits(clickHandler)
		);
		return this;
	}

	public void SoftwareClick()
	{
		button.onClick?.Invoke();
	}

	private UnityAction WithRateLimits(UnityAction action)
	{
		return new UnityAction(() => {
			TimeSpan ts = DateTime.Now - lastClick;
			if (ts.TotalMilliseconds > rateLimitMs) {
				lastClick += ts;
				action?.Invoke();
			}
		});
	}
}
