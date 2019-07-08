using UnityEngine.Events;

public interface IPopUpController
{
    void ShowPopUp(string message, PopUpWindows popUp);
    void ShowPopUp(string message, string header);
    UnityAction OnClosePopUp { set; }
    UnityAction OnReturnToLogin { set; }
}
