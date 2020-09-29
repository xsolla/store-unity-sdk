using UnityEngine;

public class SimpleButtonWithObjects : SimpleButton
{
#pragma warning disable 0649
	[SerializeField] GameObject[] NormalStateObjects;
	[SerializeField] GameObject[] HoverStateObjects;
	[SerializeField] GameObject[] PressedStateObjects;
#pragma warning restore 0649

	protected override void OnNormal()
	{
		base.OnNormal();

		EnableObjects(NormalStateObjects);
		DisableObjects(HoverStateObjects);
		DisableObjects(PressedStateObjects);
	}

	protected override void OnHover()
	{
		base.OnHover();

		DisableObjects(NormalStateObjects);
		EnableObjects(HoverStateObjects);
		DisableObjects(PressedStateObjects);
	}

	protected override void OnPressed()
	{
		base.OnPressed();

		DisableObjects(NormalStateObjects);
		DisableObjects(HoverStateObjects);
		EnableObjects(PressedStateObjects);
	}

	void EnableObjects(GameObject[] objects)
	{
		SetObjectsActive(objects, state: true);
	}

	void DisableObjects(GameObject[] objects)
	{
		SetObjectsActive(objects, state: false);
	}

	void SetObjectsActive(GameObject[] objects, bool state)
	{
		if (objects == null || objects.Length == 0)
			return;
		else
		{
			for (int i = 0; i < objects.Length; i++)
				objects[i].SetActive(state);
		}
	}
}
