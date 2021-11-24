using UnityEngine;

namespace Xsolla.Demo
{
    public class OnButtonDisabler : MonoBehaviour
    {
		[SerializeField] private SimpleButton Button;

		private void Awake()
		{
			Button.onClick += () => this.gameObject.SetActive(false);
		}
	}
}
