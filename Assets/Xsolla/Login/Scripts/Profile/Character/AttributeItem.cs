using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class AttributeItem : MonoBehaviour
	{
		[SerializeField] bool _isReadOnly;
		[SerializeField] InputField KeyInputField;
		[SerializeField] InputField ValueInputField;
		[SerializeField] Text KeyText;
		[SerializeField] Text ValueText;
		[SerializeField] SimpleButton RemoveButton;

		private string _key;
		private string _value;

		public static Action<AttributeItem, string, string> OnKeyChanged;
		public static Action<AttributeItem, string, string> OnValueChanged;
		public static Action<AttributeItem> OnRemoveRequest;

		public bool IsReadOnly
		{
			get
			{
				return _isReadOnly;
			}
			set
			{
				if (value == _isReadOnly)
					return;

				_isReadOnly = value;
				SetAccessibility(value);
			}
		}

		public string Key
		{
			get
			{
				return _key;
			}
			set
			{
				_key = value;
				KeyText.text = value;

				if (KeyInputField)
					KeyInputField.text = value;
			}
		}

		public string Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				ValueText.text = value;

				if (ValueInputField)
					ValueInputField.text = value;
			}
		}

		public bool IsPublic	{ get; set; }
		public bool IsNew		{ get; set; }
		public bool IsModified	{ get; set; }

		private void Awake()
		{
			SetAccessibility(_isReadOnly);

			if (KeyInputField)
				KeyInputField.onEndEdit.AddListener(SetNewKey);

			if (ValueInputField)
				ValueInputField.onEndEdit.AddListener(SetNewValue);

			if (RemoveButton)
				RemoveButton.onClick += () =>
				{
					if (OnRemoveRequest != null)
						OnRemoveRequest.Invoke(this);
				};
		}

		private void SetAccessibility(bool isReadOnly)
		{
			if (KeyInputField)
				KeyInputField.gameObject.SetActive(!isReadOnly);

			if (ValueInputField)
				ValueInputField.gameObject.SetActive(!isReadOnly);

			if (KeyText)
				KeyText.gameObject.SetActive(isReadOnly);

			if (ValueText)
				ValueText.gameObject.SetActive(isReadOnly);

			if (RemoveButton)
				RemoveButton.gameObject.SetActive(!isReadOnly);
		}

		private void SetNewKey(string newKey)
		{
			if(newKey == Key)
				return;

			var oldKey = Key;
			Key = newKey;

			if (OnKeyChanged != null)
				OnKeyChanged.Invoke(this, oldKey, newKey);
		}

		private void SetNewValue(string newValue)
		{
			if(newValue == Value)
				return;

			var oldValue = Value;
			Value = newValue;

			if (OnValueChanged != null)
				OnValueChanged.Invoke(this, oldValue, newValue);
		}
	}
}
