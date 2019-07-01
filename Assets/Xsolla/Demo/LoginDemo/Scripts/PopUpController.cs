using UnityEngine;
using UnityEngine.Events;

public class PopUpController : MonoBehaviour, IPopUpController
{
    [SerializeField] private GameObject success_Panel;
    [SerializeField] private GameObject error_Panel;
    [SerializeField] private GameObject warning_Panel;
    [SerializeField] private GameObject extended_Panel;

    public UnityAction OnClosePopUp
    {
        set
        {
            success_Panel.GetComponent<IPopUp>().OnClose = value;
            error_Panel.GetComponent<IPopUp>().OnClose = value;
            extended_Panel.GetComponent<IPopUp>().OnClose = value;
        }
    }
    public UnityAction OnReturnToLogin
    {
        set
        {
            extended_Panel.GetComponent<IExtendedPopUp>().OnReturnToLogin = value;
        }
    }
    public void ShowPopUp(string message, PopUpWindows popUp)
    {
        switch (popUp)
        {
            case PopUpWindows.Success:
                success_Panel.GetComponent<IPopUp>().ShowPopUp(message);
                break;

            case PopUpWindows.Error:
                error_Panel.GetComponent<IPopUp>().ShowPopUp(message);
                break;

            case PopUpWindows.Warning:
                warning_Panel.GetComponent<IPopUp>().ShowPopUp(message);
                break;
            case PopUpWindows.Extended:
                extended_Panel.GetComponent<IPopUp>().ShowPopUp(message);
                break;
        }
    }
    public void ShowPopUp(string header, string message)
    {
        extended_Panel.GetComponent<IExtendedPopUp>().ShowPopUp(header, message);
    }
}
