using System;
using UnityEngine;
using UnityEngine.UI;

public class OldAttributeItemUI : MonoBehaviour
{
	[SerializeField]
	InputField inputKey;
	[SerializeField]
	InputField inputValue;
	[SerializeField]
	SimpleButton removeButton;

	public Action<OldAttributeItemUI, string> onKeyChanged;
	public Action<OldAttributeItemUI> onValueChanged;
	public Action<OldAttributeItemUI> onRemove;
	
	private string _key;
	private string _value;

	public string Key
	{
		get => _key;
		set
		{
			_key = value;
			inputKey.text = _key;
		}
	}
	public string Value
	{
		get => _value;
		set
		{
			_value = value;
			inputValue.text = _value;
		}
	}

	public void Initialize(string key, string value)
	{
		inputKey.text = key;
		inputKey.onEndEdit.AddListener(OnKeyEdited);
		
		inputValue.text = value;
		inputValue.onEndEdit.AddListener(OnValueEdited);

		removeButton.onClick = () =>
		{
			onRemove?.Invoke(this);
			Destroy(gameObject, 0.01F);
		};
	}

	private void OnKeyEdited(string newText)
	{
		if (Key.Equals(newText)) return;
		var oldKey = Key;
		_key = newText;
		onKeyChanged?.Invoke(this, oldKey);
	}

	private void OnValueEdited(string newText)
	{
		if (Value.Equals(newText)) return;
		_value = newText;
		onValueChanged?.Invoke(this);
	}
}