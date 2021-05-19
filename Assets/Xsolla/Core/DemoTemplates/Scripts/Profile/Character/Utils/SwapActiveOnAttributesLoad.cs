using UnityEngine;

public class SwapActiveOnAttributesLoad : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] private UserAttributesProvider AttributesProvider;
	[SerializeField] private Transform AttributesParent;
	[SerializeField] private GameObject[] ActiveObjects;
	[SerializeField] private GameObject[] InactiveObjects;
#pragma warning restore 0649

	private bool IsAttributesLoaded
	{
		get
		{
			if (AttributesParent != null)
				if (AttributesParent.transform != null)
					return AttributesParent.transform.childCount > 0;

			//else
			return false;
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
