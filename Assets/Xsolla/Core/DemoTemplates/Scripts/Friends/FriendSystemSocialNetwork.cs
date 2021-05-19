using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SimpleTextButton))]
public class FriendSystemSocialNetwork : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Image logo;
    [SerializeField] private Image linkIcon;
    [SerializeField] private Image addIcon;
    [SerializeField] private SocialProvider provider = SocialProvider.None;

    public enum State
    {
        Linked,
        Unlinked
    }

    public event Action<SocialProvider, State> StateChanged;

	public SocialProvider Provider { get { return provider; } }
    public SimpleButton Button { get { return GetComponent<SimpleButton>(); } }

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

        DemoController.Instance.GetImplementation().GetLinkedSocialProviders(networks =>
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
				var message = string.Format("Try to set unknown state: '{0}'", state);
				Debug.LogWarning(message);
                return;
            }
        }

		if (StateChanged != null)
			StateChanged.Invoke(provider, state);
    }

    private void UnlinkSocialProvider()
    {
        Debug.Log("We can not unlink social provider yet. So do nothing.");
    }
    
    private void LinkSocialProvider()
    {
		Action<SocialProvider> onSuccessLink = _ => RefreshState();
        DemoController.Instance.GetImplementation().LinkSocialProvider(provider, onSuccessLink);
    }
}
