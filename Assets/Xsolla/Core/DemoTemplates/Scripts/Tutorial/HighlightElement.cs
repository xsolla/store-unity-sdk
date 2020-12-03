using UnityEngine;
using UnityEngine.UI;

public class HighlightElement : MonoBehaviour
{
	[SerializeField] private string Id;
	[SerializeField] Color highlightColor;
	public string ID => Id;

	public void Highlight()
	{
		// in case highlighted object is a button any interaction with it should be disabled
		GetComponentInChildren<SimpleTextButton>()?.Lock();

		var image = GetComponentInChildren<Image>();
		if (image != null)
			image.color = highlightColor;

		var text = GetComponentInChildren<Text>();
		if (text != null)
			text.color = highlightColor;
	}
}