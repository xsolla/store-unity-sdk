using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalSizeChanger : MonoBehaviour
{
	public RectTransform.Edge edge;

	private RectTransform rectTransform;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
 	}

	public void SetWidth(float value)
	{
		rectTransform.SetInsetAndSizeFromParentEdge(edge, 0, value);
	}

	public float GetWidth()
	{
		return rectTransform.rect.width;
	}
}
