using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using Xsolla.Core;
using Xsolla.Login;

public partial class TestHelper : MonoSingleton<TestHelper>
{
	public enum Scenes
	{
		Login,
		Store,
		PayStation,
		EmptyScene
	}

	public IEnumerator LoadScene(Scenes scene, float timeout = 5.0F)
	{
		if(scene == Scenes.Login)
			XsollaLogin.Instance.DeleteToken(Constants.LAST_SUCCESS_AUTH_TOKEN);
		if (IsScene(scene))
			yield return UnloadScene(scene);
		else
			WaitScene(GetCurrentScene(), timeout);

		SceneManager.LoadScene(GetSceneName(scene));
		yield return new WaitForSeconds(2.0F);
		yield return WaitScene(scene, timeout);
	}

	/// <summary>
	/// Костыль
	/// </summary>
	/// <param name="scene">Scene name</param>
	/// <param name="timeout">Wait timeout for unload scene</param>
	/// <returns></returns>
	public IEnumerator UnloadScene(Scenes scene, float timeout = 5.0F)
	{
		yield return WaitScene(scene, timeout);
		yield return LoadScene(Scenes.EmptyScene);
		// scene == Scenes.Login
		// ? Scenes.EmptyScene
		// : Scenes.Login);
	}

	public bool IsScene(Scenes scene)
	{
		return GetCurrentScene() == scene;
	}

	public IEnumerator WaitScene(Scenes scene, float timeout)
	{
		if (scene == Scenes.Login)
			timeout = 20.0F;
		Debug.Log($"Wait scene = `{scene.ToString()}`");
		bool busy = true;
		Coroutine timeoutCoroutine = null;
		Coroutine sceneCoroutine = StartCoroutine(WaitSceneCoroutine(scene, () =>
		{
			StopCoroutine(timeoutCoroutine);
			Debug.Log($"Waiting scene = `{scene.ToString()}` complete.");
			busy = false;
		}));
		timeoutCoroutine = StartCoroutine(WaitTimeoutCoroutine(timeout, () =>
		{
			StopCoroutine(sceneCoroutine);
			Debug.LogWarning(
				$"ATTENTION: wait scene `{scene.ToString()}` coroutine exit by timeout = {timeout.ToString(CultureInfo.CurrentCulture)}");
			busy = false;
		}));
		yield return new WaitWhile(() => busy);
	}

	IEnumerator WaitSceneCoroutine(Scenes scene, Action callback, float noRequestTimeout = 1.0F)
	{
		yield return new WaitUntil(() => IsScene(scene));
		yield return WaitWebRequests(noRequestTimeout);
		callback?.Invoke();
	}

	public IEnumerator WaitWebRequests(float noRequestTimeout = 1.0F)
	{
		yield return StartCoroutine(WaitWebRequestCoroutine(noRequestTimeout));
	}

	IEnumerator WaitWebRequestCoroutine(float noRequestTimeout)
	{
		Debug.Log($"Wait web request helper.");
		while (true)
		{
			yield return new WaitWhile(() => WebRequestHelper.Instance.IsBusy());
			var elapsed = false;
			var timeCoroutine = StartCoroutine(WaitTimeoutCoroutine(noRequestTimeout, () => elapsed = true));
			yield return new WaitUntil(() => elapsed || WebRequestHelper.Instance.IsBusy());
			if (!elapsed)
			{
				Debug.Log("Web request helper is busy.");
				StopCoroutine(timeCoroutine);
			}
			else
			{
				Debug.Log($"Web request helper is NOT busy.");
				break;
			}
		}
	}

	IEnumerator WaitTimeoutCoroutine(float timeout, Action callback = null)
	{
		yield return new WaitForSeconds(timeout);
		callback?.Invoke();
	}

	private Scenes GetCurrentScene()
	{
		string sceneName = SceneManager.GetActiveScene().name;
		return Enum.TryParse(sceneName, false, out Scenes result) ? result : Scenes.EmptyScene;
	}

	private string GetSceneName(Scenes scene)
	{
		return Enum.GetName(typeof(Scenes), scene);
	}
}