using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Core.Popup;
using Xsolla.Login;

public class UserProfileAvatarManager : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] GameObject SetBlock;
	[SerializeField] GameObject UnsetBlock;
	[SerializeField] GameObject EditBlock;

	[SerializeField] SimpleButton[] EditButtons;
	[SerializeField] SimpleButton DeleteButton;
#pragma warning restore 0649

	private void Awake()
	{
		AvatarChoiceButton.AvatarPicked += SetNewAvatar;

		foreach (var button in EditButtons)
			button.onClick += () => SetState(AvatarState.Edit);

		DeleteButton.onClick += () => SetNewAvatar(null);
	}

	private void OnDestroy()
	{
		AvatarChoiceButton.AvatarPicked -= SetNewAvatar;
	}

	private IEnumerator Start()
	{
		string pictureUrl = null;
		bool? isUserPictureUrlObtained = null;

		var token = DemoController.Instance.GetImplementation().Token;
		DemoController.Instance.GetImplementation().GetUserInfo(token,
			onSuccess: info =>
			{
				pictureUrl = info.picture;
				isUserPictureUrlObtained = true;
			},
			onError: error =>
			{
				StoreDemoPopup.ShowError(error);
				isUserPictureUrlObtained = false;
			});

		yield return new WaitWhile(() => isUserPictureUrlObtained == null);

		if (isUserPictureUrlObtained == true && !string.IsNullOrEmpty(pictureUrl))
			SetState(AvatarState.Set);
		else
			SetState(AvatarState.Unset);
	}

	private void SetNewAvatar(Sprite newAvatar)
	{
		if (newAvatar != null)
		{
			UploadPicture(newAvatar);
			SetState(AvatarState.Set);
		}
		else
		{
			DeletePicture();
			SetState(AvatarState.Unset);
		}
	}

	private void SetState(AvatarState avatarState)
	{
		if (SetBlock)
			SetBlock.SetActive(avatarState == AvatarState.Set);
		if (UnsetBlock)
			UnsetBlock.SetActive(avatarState == AvatarState.Unset);
		if (EditBlock)
			EditBlock.SetActive(avatarState == AvatarState.Edit);
	}

	private void UploadPicture(Sprite sprite)
	{
		bool? isImageUploaded = null;
		ShowWaiting(() => isImageUploaded == null);

		string boundary;
		byte[] data = ConvertToData(sprite, out boundary);

		var token = DemoController.Instance.GetImplementation().Token;
		DemoController.Instance.GetImplementation().UploadUserPicture(token, data, boundary,
			onSuccess: imageInfo =>
			{
				var packedInfo = ParseUtils.FromJson<UserImageUpload>(imageInfo);

				if (packedInfo != null)
				{
					var pictureUrl = packedInfo.picture;
					SetPictureUrlToInfo(pictureUrl,
						onSuccess: () =>
						{
							ImageLoader.Instance.AddImage(pictureUrl, sprite);
							RefreshImageLoaders();
							isImageUploaded = true;
						},
						onError: () => isImageUploaded = false);
				}
				else
					Debug.LogError(string.Format("Could not parse server response: {0}", imageInfo));
			},
			onError: error =>
			{
				isImageUploaded = false;
				StoreDemoPopup.ShowError(error);
			});
	}

	private void DeletePicture()
	{
		bool? isPictureDeleted = null;
		ShowWaiting(() => isPictureDeleted == null);

		var token = DemoController.Instance.GetImplementation().Token;
		DemoController.Instance.GetImplementation().DeleteUserPicture(token,
			onSuccess: () =>
			{
				SetPictureUrlToInfo(null,
					onSuccess: () =>
					{
						isPictureDeleted = true;
						RefreshImageLoaders();
					},
					onError: () => isPictureDeleted = false);
			},
			onError: error =>
			{
				isPictureDeleted = false;
				StoreDemoPopup.ShowError(error);
			});
	}

	private void ShowWaiting(Func<bool> waitWhile)
	{
		PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => !waitWhile.Invoke());
	}

	private void SetPictureUrlToInfo(string pictureUrl, Action onSuccess = null, Action onError = null)
	{
		var token = DemoController.Instance.GetImplementation().Token;
		DemoController.Instance.GetImplementation().GetUserInfo(token,
			onSuccess: info =>
			{
				info.picture = pictureUrl;
				if (onSuccess != null)
					onSuccess.Invoke();
			},
			onError: error =>
			{
				StoreDemoPopup.ShowError(error);
				if (onError != null)
					onError.Invoke();
			});
	}

	private byte[] ConvertToData(Sprite sprite, out string boundary)
	{
		boundary = string.Format("{0}{1}", new string('-', 27), DateTime.Now.Ticks);
		var beginBoundary = string.Format("\r\n--{0}\r\n", boundary);
		var endBoundary = string.Format("\r\n--{0}--\r\n", boundary);

		var texture = sprite.texture;
		var binaryData = texture.EncodeToPNG();

		var pictureHeader = "Content-Disposition: form-data;name=\"picture\";filename=\"avatar.png\"\r\nContent-Type: \r\n\r\n";

		List<byte> uploadContent = new List<byte>();
		uploadContent.AddRange(Encoding.Default.GetBytes(beginBoundary));
		uploadContent.AddRange(Encoding.Default.GetBytes(pictureHeader));
		uploadContent.AddRange(binaryData);
		uploadContent.AddRange(Encoding.Default.GetBytes(endBoundary));

		return uploadContent.ToArray();
	}

	private void RefreshImageLoaders()
	{
		var avatarLoaders = FindObjectsOfType<UserAvatarLoader>();

		if (avatarLoaders != null)
			foreach (var loader in avatarLoaders)
			{
				if (loader)
					loader.Refresh();
			}
	}

	private enum AvatarState
	{
		Set, Unset, Edit
	}
}
