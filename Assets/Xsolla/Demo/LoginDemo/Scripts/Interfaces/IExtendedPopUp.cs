using UnityEngine.Events;

public interface IExtendedPopUp
{
    void ShowPopUp(string header, string message);
    UnityAction OnReturnToLogin { set; }
}
