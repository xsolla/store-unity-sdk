using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Login;

public class AttributesSidePanelItem : MonoBehaviour
{
	[SerializeField]
	Text attributeKey;
	[SerializeField]
	Text attributeValue;
	
	public void Initialize(UserAttributeModel itemInformation)
	{
		attributeKey.text = itemInformation.key;
		attributeValue.text = itemInformation.value;
	}
}