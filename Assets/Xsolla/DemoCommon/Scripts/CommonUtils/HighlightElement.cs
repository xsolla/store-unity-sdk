using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class HighlightElement : MonoBehaviour
	{
		[SerializeField] private string Id;
		[SerializeField] Color highlightColor;
		public string ID
		{
			get
			{
				return Id;
			}
		}

		public void Highlight()
		{
			// in case highlighted object is a button any interaction with it should be disabled
			var button = GetComponentInChildren<SimpleTextButton>();
			if (button != null)
				button.Lock();

			var image = GetComponentInChildren<Image>();
			if (image != null)
				image.color = highlightColor;

			var text = GetComponentInChildren<Text>();
			if (text != null)
				text.color = highlightColor;
		}
	}
}
