using System;
using UnityEngine;
using UnityEngine.UI;

public class FriendSearchBox : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] private InputField SearchInputField;
	[SerializeField] private SimpleButton ClearButton;
	[SerializeField] private SimpleButton AdditionalClearButton;
	[SerializeField] private SimpleButton SearchButton;
#pragma warning restore 0649

	public event Action ClearSearchRequest;
	public event Action<string> SearchRequest;

	private void Awake()
	{
		SearchInputField.onValueChanged.AddListener(ShowHideClearButton);
		SearchInputField.onEndEdit.AddListener(ProcessHotkeys);
		ClearButton.onClick += ClearSearch;
		AdditionalClearButton.onClick += ClearSearch;
		SearchButton.onClick += RequestSearch;
	}

	private void ShowHideClearButton(string userInput)
	{
		if (string.IsNullOrEmpty(userInput))
			ClearButton.gameObject.SetActive(false);
		else if (ClearButton.gameObject.activeSelf != true)
			ClearButton.gameObject.SetActive(true);
	}

	private void ClearSearch()
	{
		SearchInputField.text = string.Empty;
		ClearSearchRequest?.Invoke();
	}

	private void RequestSearch()
	{
		var userInput = SearchInputField.text;
		
		if (!string.IsNullOrEmpty(userInput))
			SearchRequest?.Invoke(userInput);
		else
			ClearSearchRequest?.Invoke();
	}

	private void ProcessHotkeys(string _)
	{
		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
			RequestSearch();
		else if (Input.GetKeyDown(KeyCode.Escape))
			ClearSearch();
	}
}
