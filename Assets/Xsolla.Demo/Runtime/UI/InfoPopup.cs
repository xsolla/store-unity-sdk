using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class InfoPopup : Screen
	{
		[field: SerializeField] private TMP_Text TitleLabel { get; set; }
		[field: SerializeField] private TMP_Text MessageLabel { get; set; }
		[field: SerializeField] private Button CloseButton { get; set; }

		private ScreenService ScreenService => ServiceLocator.Resolve<ScreenService>();

		private Action CloseCallback { get; set; }

		private void Start()
		{
			CloseButton.onClick.AddListener(() => {
				ScreenService.Close(this);
				CloseCallback?.Invoke();
			});
		}

		private void OnDestroy()
		{
			CloseButton.onClick.RemoveAllListeners();
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.KeypadEnter)
				|| Input.GetKeyDown(KeyCode.Return)
				|| Input.GetKeyDown(KeyCode.Escape))
			{
				if (CloseButton.interactable)
					CloseButton.onClick.Invoke();
			}
		}

		public InfoPopup SetCloseCallback(Action value)
		{
			CloseCallback = value;
			return this;
		}

		public InfoPopup SetTitle(string value)
		{
			TitleLabel.text = value;
			return this;
		}

		public InfoPopup SetMessage(string value)
		{
			MessageLabel.text = value;
			return this;
		}
	}
}