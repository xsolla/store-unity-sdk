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
	
	UserAttribute _itemInformation;
	
	public void Initialize(UserAttribute itemInformation)
	{
		_itemInformation = itemInformation;

		attributeKey.text = itemInformation.key;
		attributeValue.text = itemInformation.value;
	}
}