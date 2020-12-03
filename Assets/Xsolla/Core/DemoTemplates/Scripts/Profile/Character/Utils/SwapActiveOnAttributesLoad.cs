using UnityEngine;

namespace Xsolla.Demo
{
	public class SwapActiveOnAttributesLoad : MonoBehaviour
	{
		[SerializeField] private UserAttributesProvider AttributesProvider = default;
		[SerializeField] private Transform AttributesParent = default;
		[SerializeField] private GameObject[] ActiveObjects = default;
		[SerializeField] private GameObject[] InactiveObjects = default;

		private bool IsAttributesLoaded => AttributesParent?.transform?.childCount > 0;

		private void Awake()
		{
			if (/*already*/IsAttributesLoaded)
				SwapActive();

			AttributesProvider.OnSuccess += SwapActive;
		}

		private void OnDestroy()
		{
			AttributesProvider.OnSuccess -= SwapActive;
		}

		private void SwapActive()
		{
			if (IsAttributesLoaded)
			{
				foreach (var item in ActiveObjects)
					item.SetActive(false);

				foreach (var item in InactiveObjects)
					item.SetActive(true);
			}
		}
	}
}
