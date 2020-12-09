using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	[RequireComponent(typeof(SimpleTextButton))]
	public class FriendSystemSocialNetwork : MonoBehaviour
	{
		[SerializeField] private Image background = default;
		[SerializeField] private Image logo = default;
		[SerializeField] private Image linkIcon = default;
		[SerializeField] private Image addIcon = default;
		[SerializeField] private SocialProvider provider = SocialProvider.None;

		public enum State
		{
			Linked,
			Unlinked
		}

		public event Action<SocialProvider, State> StateChanged;

		public SocialProvider Provider => provider;
		public SimpleButton Button => GetComponent<SimpleButton>();

		private void Awake()
		{
			SetState(State.Unlinked);
		}

		private void Start()
		{
			RefreshState();
		}

		void RefreshState()
		{
			if (provider == SocialProvider.None)
				return;

			DemoController.Instance.LoginDemo.GetLinkedSocialProviders(networks =>
			{
				if (networks.Any(n => n.provider.Equals(provider.GetParameter())))
					SetState(State.Linked);
				else
					SetState(State.Unlinked);
			});
		}

		private void SetVisibility(MonoBehaviour component, bool visibility)
		{
			if(component != null)
				component.gameObject.SetActive(visibility);
		}

		private void SetImageOpacity(Image image, float opacity)
		{
			var tmpColor = image.color;
			tmpColor.a = opacity;
			image.color = tmpColor;
		}

		public void SetState(State state)
		{
			switch (state)
			{
				case State.Linked:
				{
					SetVisibility(background, true);
					SetVisibility(logo, true);
					SetVisibility(linkIcon, true);
					SetVisibility(addIcon, false);

					SetImageOpacity(logo, 0.7F);
					GetComponent<SimpleTextButton>().onClick = UnlinkSocialProvider;
					break;
				}
				case State.Unlinked:
				{
					SetVisibility(background, false);
					SetVisibility(logo, true);
					SetVisibility(linkIcon, false);
					SetVisibility(addIcon, true);

					SetImageOpacity(logo, 1.0F);
					GetComponent<SimpleTextButton>().onClick = LinkSocialProvider;
					break;
				}
				default:
				{
					Debug.LogWarning($"Try to set unknown state: '{state}'");
					return;
				}
			}
			StateChanged?.Invoke(provider, state);
		}

		private void UnlinkSocialProvider()
		{
			Debug.Log("We can not unlink social provider yet. So do nothing.");
		}

		private void LinkSocialProvider()
		{
			Action<SocialProvider> onSuccessLink = _ => RefreshState();
			DemoController.Instance.LoginDemo.LinkSocialProvider(provider, onSuccessLink);
		}
	}
}
