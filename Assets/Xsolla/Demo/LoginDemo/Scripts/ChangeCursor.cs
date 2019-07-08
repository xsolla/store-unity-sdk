using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private CursorMode cursorMode ;
    [SerializeField] private Vector2 hotSpot;
    private bool isOver = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
    private void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
    private void OnEnable()
    {
        if (isOver)
            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }
}
