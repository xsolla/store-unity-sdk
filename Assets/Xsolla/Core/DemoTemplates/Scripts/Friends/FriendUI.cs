using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

public class FriendUI : MonoBehaviour
{
    [SerializeField] private Image avatarImage;
    [SerializeField] private Image statusImage;
    [SerializeField] private Text nickname;
    [SerializeField] private SimpleButton actionsButton;
    [SerializeField] private GameObject actionsList;
    [SerializeField] private FriendButtonsUI userButtons;
    
    public void Initialize(FriendModel friend)
    {
        if(friend == null) return;

        InitAvatar(friend);

        InitStatus(friend);

        InitNickname(friend);

        InitActionsButton(friend);
    }

    private void InitAvatar(FriendModel friend)
    {
        if (!string.IsNullOrEmpty(friend.AvatarUrl) && avatarImage != null)
        {
            ImageLoader.Instance.GetImageAsync(friend.AvatarUrl, (url, sprite) =>
            {
                avatarImage.gameObject.SetActive(true);
                avatarImage.sprite = sprite;
            });
        }
    }

    private void InitStatus(FriendModel friend)
    {
        if (statusImage != null)
        {
            statusImage.gameObject.SetActive(true);
            switch (friend.Status)
            {
                case UserOnlineStatus.Online:
                    statusImage.color = new Color(0.043F, 0.043F, 0, 1.0F);
                    break;
                case UserOnlineStatus.Offline:
                    statusImage.color = new Color(0.517F, 0.557F, 0.694F, 0.5F);
                    break;
                case UserOnlineStatus.Unknown:
                    statusImage.color = new Color(0.5F, 0.5F, 0.5F, 0.5F);
                    break;
                default:
                    statusImage.color = new Color(0.8F, 0F, 0F, 0.3F);
                    break;
            }
        }
    }

    private void InitNickname(FriendModel friend)
    {
        var text = string.IsNullOrEmpty(friend.Nickname) ? string.Empty : friend.Nickname;
        if(!string.IsNullOrEmpty(text))
            gameObject.name = text;
        if (nickname != null)
            nickname.text = text;
    }

    private void InitActionsButton(FriendModel friendModel)
    {
        if (actionsButton != null && actionsList != null)
        {
            actionsList.GetComponent<FriendActionsUI>().Init(friendModel, userButtons);
            actionsList.gameObject.SetActive(false);
            actionsButton.onClick = () =>
            {
                actionsList.SetActive(!actionsList.activeInHierarchy);
            };
        }
    }
}
