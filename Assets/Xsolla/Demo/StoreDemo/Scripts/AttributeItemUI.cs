using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Login;

public class AttributeItemUI : MonoBehaviour
{
	[SerializeField]
	InputField inputKey;
	[SerializeField]
	InputField inputValue;
	[SerializeField]
	SimpleButton removeButton;

	private UserAttribute _itemInformation;

	public Action<string> onRemove;
	
	public void Initialize(UserAttribute itemInformation)
	{
		_itemInformation = itemInformation;
		
		inputKey.text = itemInformation.key;
		inputValue.text = itemInformation.value;

		removeButton.onClick = () => { onRemove?.Invoke(inputKey.text); };
	}
	
	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}
}