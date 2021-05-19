using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

public class UserAvatarLoader : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] private Image UserAvatarHolder;
	[SerializeField] private Sprite AvatarPlaceholder;
#pragma warning restore 0649

	private static string _expectedAvatarUrl;

	public void Refresh() { Start(); }

	private void Start()
	{
		var token = DemoController.Instance.GetImplementation().Token;
		DemoController.Instance.GetImplementation().GetUserInfo(token, info =>
		{
			DrawAvatar(info.picture);
		});
	}

	private void DrawAvatar(string avatarUrl)
	{
		if (string.IsNullOrEmpty(avatarUrl))
		{
			if (UserAvatarHolder)
				UserAvatarHolder.sprite = AvatarPlaceholder;

			_expectedAvatarUrl = null;
		}
		else
		{
			if (avatarUrl != _expectedAvatarUrl)
				_expectedAvatarUrl = avatarUrl;

			ImageLoader.Instance.GetImageAsync(avatarUrl,
				callback: (receivedAvatarUrl, avatar) =>
				{
					if (receivedAvatarUrl == _expectedAvatarUrl && UserAvatarHolder != null)
						UserAvatarHolder.sprite = avatar;
				});
		}
	}
}
