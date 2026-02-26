using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	internal class TabNavigation : MonoBehaviour
	{
		[field: SerializeField] private TMP_InputField[] InputFields { get; set; }
		[field: SerializeField] private Button SubmitButton { get; set; }

		private bool IsInteractable => SubmitButton && SubmitButton.interactable;

		private void OnEnable()
		{
			StartCoroutine(FocusFirst());
		}

		private void OnGUI()
		{
			if (!IsInteractable)
				return;
			
			var currentEvent = Event.current;
			if (currentEvent == null)
				return;

			if (currentEvent.type != EventType.KeyDown)
				return;

			if (currentEvent.keyCode != KeyCode.Tab)
				return;

			var backwards = currentEvent.shift;
			currentEvent.Use();
			Move(backwards);
		}

		private void Update()
		{
			if (!IsInteractable)
				return;

			if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
				TrySubmit();
		}

		private void Move(bool backwards)
		{
			if (InputFields == null || InputFields.Length == 0)
				return;

			var eventSystem = EventSystem.current;
			if (!eventSystem)
				return;

			var currentIndex = GetCurrentIndex(eventSystem.currentSelectedGameObject);
			if (currentIndex < 0) currentIndex = 0;

			var nextIndex = backwards
				? (currentIndex - 1 + InputFields.Length) % InputFields.Length
				: (currentIndex + 1) % InputFields.Length;

			SelectField(nextIndex);
		}

		private IEnumerator FocusFirst()
		{
			yield return null;

			var eventSystem = EventSystem.current;
			if (!eventSystem)
				yield break;

			SelectField(0);
		}

		private int GetCurrentIndex(GameObject selectedGo)
		{
			if (!selectedGo)
				return -1;

			var field = selectedGo.GetComponent<TMP_InputField>();
			if (!field)
				return -1;

			return Array.IndexOf(InputFields, field);
		}

		private void SelectField(int index)
		{
			if (InputFields == null || InputFields.Length == 0)
				return;

			index = Mathf.Clamp(index, 0, InputFields.Length - 1);

			var field = InputFields[index];
			var eventSystem = EventSystem.current;
			if (eventSystem)
				eventSystem.SetSelectedGameObject(field.gameObject);

			field.ActivateInputField();
		}

		private void TrySubmit()
		{
			var eventSystem = EventSystem.current;
			if (!eventSystem)
				return;
			
			var currentSelected = eventSystem.currentSelectedGameObject;
			if (!currentSelected)
				return;

			var currentField = currentSelected.GetComponent<TMP_InputField>();
			if (!currentField)
				return;

			if (Array.IndexOf(InputFields, currentField) < 0)
				return;

			currentField.DeactivateInputField();
			SubmitButton.onClick.Invoke();
			
			if (eventSystem)
				eventSystem.SetSelectedGameObject(null);
		}
	}
}