using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Xsolla.Core;

public partial class TestHelper : MonoSingleton<TestHelper>
{
	public void LoadScene(Scenes name)
	{
		SceneManager.LoadScene(Enum.GetName(typeof(Scenes), name));
	}

	public GameObject Find(string name)
	{
		return GameObject.Find(name);
	}

	public T Find<T>(string name) where T : Component
	{
		GameObject go = Find(name);
		return go.GetComponent<T>();
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
}

public partial class TestHelper : MonoSingleton<TestHelper>
{
	public enum Scenes
	{
		Login,
		Store,
		PayStation
	}
}