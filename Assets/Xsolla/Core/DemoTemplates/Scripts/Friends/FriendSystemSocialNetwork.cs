using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SimpleTextButton))]
public class FriendSystemSocialNetwork : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Image logo;
    [SerializeField] private Image linkIcon;
    [SerializeField] private Image addIcon;
    [SerializeField] private SocialProvider provider;

    public enum State
    {
        Linked,
        Unlinked
    }

    private void Awake()
    {
        SetState(State.Unlinked);
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
                GetComponent<SimpleTextButton>().onClick = LinkedStateButtonHandler;
                break;
            }
            case State.Unlinked:
            {
                SetVisibility(background, false);
                SetVisibility(logo, true);
                SetVisibility(linkIcon, false);
                SetVisibility(addIcon, true);
                
                SetImageOpacity(logo, 1.0F);
                GetComponent<SimpleTextButton>().onClick = UnlinkedStateButtonHandler;
                break;
            }
            default:
            {
                Debug.LogWarning($"Try to set state = '{state}'");
                break;
            }
        }
    }

    private void LinkedStateButtonHandler()
    {
        Debug.Log("We can not unlink social provider yet. So do nothing.");
    }
    
    private void UnlinkedStateButtonHandler()
    {
        DemoController.Instance.GetImplementation().LinkSocialProvider(provider);
    }
}
