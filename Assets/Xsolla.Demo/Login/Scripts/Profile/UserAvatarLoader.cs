using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class UserAvatarLoader : MonoBehaviour
	{
		[SerializeField] private Image UserAvatarHolder = default;
		[SerializeField] private Sprite AvatarPlaceholder = default;

		private static string _expectedAvatarUrl;

		public void Refresh() => Start();

		private void Start()
		{
			var token = Token.Instance;
			SdkAuthLogic.Instance.GetUserInfo(token, info =>
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
}
