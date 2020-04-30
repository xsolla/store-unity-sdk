using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Xsolla.Core;

public partial class TestHelper : MonoSingleton<TestHelper>
{
	public IEnumerator LoadScene(Scenes scene)
	{
        if (IsScene(scene))
            yield return UnloadScene(scene);
		SceneManager.LoadScene(GetSceneName(scene));
        yield return new WaitWhile(() => IsScene(scene));
        yield return new WaitForSeconds(0.5F);
        yield return new WaitWhile(() => WebRequestHelper.Instance.IsBusy());
    }

    /// <summary>
    /// Костыль
    /// </summary>
    /// <param name="scene"></param>
    /// <returns></returns>
    public IEnumerator UnloadScene(Scenes scene)
    {
        yield return LoadScene((scene == Scenes.Login)
            ? Scenes.EmptyScene
            : Scenes.Login);
    }

    public bool IsScene(Scenes scene)
    {
        return SceneManager.GetActiveScene() == SceneManager.GetSceneByName(GetSceneName(scene));
    }

    public IEnumerator WaitScene(Scenes scene, float timeout)
    {
	    bool busy = true;
	    Action callback = () =>
	    {
		    StopAllCoroutines();
		    busy = false;
	    };
	    StartCoroutine(WaitSceneCoroutine(scene, callback));
	    StartCoroutine(TimeoutCoroutine(timeout, callback));
	    yield return new WaitWhile(() => busy);
    }

    IEnumerator WaitSceneCoroutine(Scenes scene, Action callback)
    {
	    yield return new WaitUntil(() => IsScene(scene));
	    callback?.Invoke();
    }

    IEnumerator TimeoutCoroutine(float timeout, Action callback)
    {
	    yield return new WaitForSeconds(timeout);
	    callback?.Invoke();
    }

    private string GetSceneName(Scenes scene)
    {
        return Enum.GetName(typeof(Scenes), scene);
    }

    public List<GameObject> FindAll(string name)
    {
	    return FindObjectsOfType<GameObject>().
		    Where(o => o.name.Equals(name)).
		    Where(o => o.activeInHierarchy).ToList();
    }
    
    public List<T> FindAll<T>(string name) where T: Component
    {
	    List<T> result = new List<T>();
	    List<GameObject> objects = FindAll(name);
	    objects.ForEach(o => result.AddRange(SearchComponentsRecursive<T>(o)));
	    return result;
    }
    
    private List<GameObject> SearchGameObjectRecursive(GameObject go, string objectName)
    {
	    if(go.name.Equals(objectName))return new List<GameObject>{go};
	    List<GameObject> result = new List<GameObject>();
	    for (var i = 0; i < go.transform.childCount; i++)
		    result.AddRange(SearchGameObjectRecursive(go.transform.GetChild(i).gameObject, objectName));
	    return result;
    }

    private List<T> SearchComponentsRecursive<T>(GameObject go) where T : Component
    {
	    List<T> result = new List<T>();
	    
	    var component = go.GetComponent<T>();
	    if (component != null)
		    result.Add(component);
	    
	    for (var i = 0; i < go.transform.childCount; i++)
	    {
		    result.AddRange(SearchComponentsRecursive<T>(go.transform.GetChild(i).gameObject));
	    }
	    return result;
    }

    public GameObject Find(string name)
    {
	    List<GameObject> objects = FindAll(name);
	    return (objects.Count > 0) ? objects.First() : null;
	}

    public T FindIn<T>(GameObject where, string what) where T: Component
    {
	    List<GameObject> objects = SearchGameObjectRecursive(where, what);
	    List<T> components = new List<T>();
		objects.ForEach(o => components.AddRange(SearchComponentsRecursive<T>(o)));
	    return (components.Count > 0) ? components.First() : null;
    }

	public T Find<T>(string name) where T : Component
	{
		List<T> components = FindAll<T>(name);
		return (components.Count > 0) ? components.First() : null;
	}

    public void FreezeFor(int milliSeconds)
	{
		Thread.Sleep(milliSeconds);
	}

	public void FreezeFor(float seconds)
	{
		FreezeFor((int)(seconds * 1000.0F));
	}

	public IEnumerator WaitFor(float seconds)
	{
		yield return new WaitForSeconds(seconds);
	}

	public bool SetInputField(string name, string text)
	{
		InputField input = Find<InputField>(name);
		if(input != null) {
			input.text = text;
			return true;
		}
		return false;
	}

	public bool ClickButton(string name)
	{
		Button button = Find<Button>(name);
		if(button != null) {
			button.onClick.Invoke();
			return true;
		}
		return true;
	}

	public bool ClickMenuButton(string name)
	{
		MenuButton button = Find<MenuButton>(name);
		if (button != null) {
			button.onClick.Invoke(name);
			return true;
		}
		return true;
	}

	public bool ClickSimpleTextButton(string name)
	{
		SimpleTextButton button = Find<SimpleTextButton>(name);
		if (button != null) {
			button.onClick.Invoke();
			return true;
		}
		return true;
	}

    public bool ClickSimpleButton(string name)
    {
        SimpleButton button = Find<SimpleButton>(name);
        if (button != null)
        {
            button.onClick.Invoke();
            return true;
        }
        return true;
    }
}

public partial class TestHelper : MonoSingleton<TestHelper>
{
	public enum Scenes
	{
		Login,
		Store,
		PayStation,
        EmptyScene
	}
}


