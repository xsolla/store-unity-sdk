using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class FriendSearchBox : MonoBehaviour
	{
		[SerializeField] private InputField SearchInputField;
		[SerializeField] private SimpleButton ClearButton;
		[SerializeField] private SimpleButton AdditionalClearButton;
		[SerializeField] private SimpleButton SearchButton;

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
			if (ClearSearchRequest != null)
				ClearSearchRequest.Invoke();
		}

		private void RequestSearch()
		{
			var userInput = SearchInputField.text;
		
			if (!string.IsNullOrEmpty(userInput))
			{
				if (SearchRequest != null)
					SearchRequest.Invoke(userInput);
			}
			else
			{
				if (ClearSearchRequest != null)
					ClearSearchRequest.Invoke();
			}
		}

		private void ProcessHotkeys(string _)
		{
			if (InputProxy.GetKeyDown(KeyCode.Return) || InputProxy.GetKeyDown(KeyCode.KeypadEnter))
				RequestSearch();
			else if (InputProxy.GetKeyDown(KeyCode.Escape))
				ClearSearch();
		}
	}
}
