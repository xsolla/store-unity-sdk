using TMPro;
using UnityEngine;

namespace Xsolla.TechDemo.Ui
{
	public class ProcessWidget : MonoBehaviour
	{
		[field: SerializeField] private Canvas Canvas { get; set; }
		[field: SerializeField] private TMP_Text MessageText { get; set; }
		[field: SerializeField] private RectTransform Circle { get; set; }
		[field: SerializeField] private float Speed { get; set; } = 60.0f;

		private void Awake()
		{
			gameObject.SetActive(true);
			Canvas.enabled = false;
		}

		private void Update()
		{
			var delta = Vector3.forward * Speed * Time.deltaTime;
			Circle.Rotate(delta);
		}

		public void Open(string message)
		{
			MessageText.text = message;
			Canvas.enabled = true;
		}

		public void Close()
		{
			Canvas.enabled = false;
		}
	}
}