using TMPro;
using UnityEngine;

namespace Xsolla.Demo
{
	public class TabButton : MonoBehaviour
	{
		[field: SerializeField] private GameObject[] ActiveObjects { get; set; }
		[field: SerializeField] private GameObject[] InactiveObjects { get; set; }
		[field: SerializeField] private TMP_Text LabelText { get; set; }
		[field: SerializeField] private Color ActiveColor { get; set; }
		[field: SerializeField] private Color InactiveColor { get; set; }

		public void SwitchState(bool isActive)
		{
			foreach (var activeObject in ActiveObjects)
				activeObject.SetActive(isActive);

			foreach (var inactiveObject in InactiveObjects)
				inactiveObject.SetActive(!isActive);

			LabelText.color = isActive ? ActiveColor : InactiveColor;
		}
	}
}