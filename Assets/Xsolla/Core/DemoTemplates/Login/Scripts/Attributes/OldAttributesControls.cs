using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldAttributesControls : MonoBehaviour
{
	[SerializeField]
	SimpleButton newButton;
	[SerializeField]
	SimpleButton saveButton;

	public Action OnNewAttribute
	{
		set { newButton.onClick = value; }
	}
	
	public Action OnSaveAttributes
	{
		set { saveButton.onClick = value; }
	}
}