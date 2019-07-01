using UnityEngine;
public class Page : MonoBehaviour, IPage
{
    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }
}