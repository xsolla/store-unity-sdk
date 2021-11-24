using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class SocialNetworksWidget : MonoBehaviour
	{
		[SerializeField] private InputField FilterInput;
		[SerializeField] private SocialProviderButton[] SocialNetworkButtons;

		public Action<SocialProvider> OnSocialButtonClick { get; set; }

		private void Awake()
		{
			FilterInput.onValueChanged.AddListener(OnFilterChanged);
		}

		private void OnEnable()
		{
			FilterInput.onValueChanged.AddListener(OnFilterChanged);

			foreach (var button in SocialNetworkButtons)
			{
				button.OnClick = () =>
				{
					if (OnSocialButtonClick != null)
						OnSocialButtonClick.Invoke(button.SocialProvider);
				};
			}
		}

		private void OnDisable()
		{
			FilterInput.onValueChanged.RemoveListener(OnFilterChanged);
			FilterInput.text = string.Empty;
			OnFilterChanged(FilterInput.text);
		}

		private void OnFilterChanged(string filterText)
		{
			filterText = filterText.ToLower();

			foreach (var button in SocialNetworkButtons)
			{
				button.gameObject.SetActive(button.SocialProvider.GetParameter().StartsWith(filterText));
			}
		}
	}
}
