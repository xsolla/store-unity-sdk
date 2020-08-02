using UnityEngine;

public class SwapActiveOnAttributesLoad : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] private UserAttributesProvider AttributesProvider;
	[SerializeField] private Transform AttributesParent;
	[SerializeField] private GameObject ActiveObject;
	[SerializeField] private GameObject InactiveObject;
#pragma warning restore 0649

	private bool IsAttributesLoaded => AttributesParent.transform.childCount > 0;

	private void Awake()
	{
		if (/*already*/IsAttributesLoaded)
			SwapActive();

		AttributesProvider.OnSuccess += SwapActive;
	}

	private void SwapActive()
	{
		if (IsAttributesLoaded)
		{
			ActiveObject.SetActive(false);
			InactiveObject.SetActive(true);
		}
	}
}
