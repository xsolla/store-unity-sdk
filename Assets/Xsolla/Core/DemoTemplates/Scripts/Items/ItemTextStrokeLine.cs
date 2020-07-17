using UnityEngine;
using UnityEngine.UI;

public class ItemTextStrokeLine : MonoBehaviour
{
    [SerializeField] private Text text;
    private Image _image;
    private RectTransform rect;
    private void Awake()
    {
        rect = (RectTransform) transform;
        _image = GetComponent<Image>();
    }

    void Update()
    {
        var prefWidth = text.preferredWidth;
        if (prefWidth > 28)
        {
            var k = 0.01F;
            if (prefWidth > 40)
                k = 0.011F;
            if (prefWidth > 50)
                k = 0.010F;
            if (prefWidth > 60)
                k = 0.015F;
            prefWidth /= 1.1F + (prefWidth - 28.2F)*k;
        }
        rect.sizeDelta = new Vector2(prefWidth, rect.sizeDelta.y);
        rect.anchoredPosition = new Vector2(prefWidth / 2, rect.anchoredPosition.y);
    }
}
