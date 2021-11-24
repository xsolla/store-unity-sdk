using UnityEngine;

namespace Xsolla.Demo
{
	public class SwapActiveOnAttributesLoad : MonoBehaviour
	{
		[SerializeField] private UserAttributesProvider AttributesProvider;
		[SerializeField] private Transform AttributesParent;
		[SerializeField] private GameObject[] ActiveObjects;
		[SerializeField] private GameObject[] InactiveObjects;

		private bool IsAttributesLoaded
		{
			get
			{
				if (!AttributesParent ||
					!AttributesParent.transform) return false;

				return AttributesParent.transform.childCount > 0;
			}
		}

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
