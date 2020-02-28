using System;
using System.Collections;
using UnityEngine;

public class HotkeyCoroutine : MonoBehaviour
{
    public event Action KeyPressedEvent;
	private KeyCode KeyCode;
	private float Timeout;
	private Coroutine coroutine;

	public HotkeyCoroutine StartCoroutine(KeyCode keyCode, float timeout = 0.4F)
	{
		KeyCode = keyCode;
		Timeout = timeout;
		stopCoroutine();
		startCoroutine();
		return this;
	}

	private void OnEnable()
	{
		startCoroutine();
	}

	private void OnDisable()
	{
		stopCoroutine();
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
	}

	void startCoroutine()
	{
		if (coroutine == null) {
			coroutine = StartCoroutine(SomeHotkeyCoroutine());
		}
	}

	void stopCoroutine()
	{
		if (coroutine != null) {
			StopCoroutine(coroutine);
			coroutine = null;
		}
	}

	IEnumerator SomeHotkeyCoroutine()
	{
		while (true) {
			yield return new WaitForSeconds(Timeout);
			yield return new WaitUntil(() => Input.GetKeyDown(KeyCode));
			KeyPressedEvent?.Invoke();
		}
	}
}
