using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.TechDemo.Ui
{
	public class PopupWidget : MonoBehaviour
	{
		[field: SerializeField] private Canvas Canvas { get; set; }
		[field: SerializeField] private TMP_Text TitleText { get; set; }
		[field: SerializeField] private TMP_Text MessageText { get; set; }
		[field: SerializeField] private Button CloseButton { get; set; }

		private void Awake()
		{
			gameObject.SetActive(true);
			Canvas.enabled = false;
		}

		private void Start()
		{
			CloseButton.onClick.AddListener(Close);
		}

		private void OnDestroy()
		{
			CloseButton.onClick.RemoveListener(Close);
		}

		public void Open(string title, string message)
		{
			TitleText.text = title;
			MessageText.text = message;
			Canvas.enabled = true;
		}

		private void Close()
		{
			Canvas.enabled = false;
		}
	}
}