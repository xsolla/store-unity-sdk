using UnityEngine.Events;

public interface IPopUp
{
    void ShowPopUp(string message);
    UnityAction OnClose { set; }
}
