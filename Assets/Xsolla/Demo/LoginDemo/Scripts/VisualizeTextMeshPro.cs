using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class VisualizeTextMeshPro : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Color normal_Color;
    [SerializeField] private Color hover_Color;
    private TextMeshProUGUI textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        textMesh.color = hover_Color;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        textMesh.color = normal_Color;
    }
}
