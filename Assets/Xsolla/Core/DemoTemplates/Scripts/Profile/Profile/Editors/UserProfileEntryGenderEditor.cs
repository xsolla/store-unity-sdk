using UnityEngine;
using UnityEngine.UI;

public class UserProfileEntryGenderEditor : UserProfileEntryEditor
{
#pragma warning disable 0649
	[SerializeField] Toggle MaleCheckbox;
	[SerializeField] Toggle FemaleCheckbox;
#pragma warning restore 0649

	private bool _skipNextCheck;

	public override void SetInitial(string value)
	{
		if (string.IsNullOrEmpty(value))
			return;

		bool? isMale = null;

		if (value == UserProfileGender.MALE)
			isMale = true;
		else if (value == UserProfileGender.FEMALE)
			isMale = false;

		if (isMale != null)
		{
			_skipNextCheck = true;

			if (isMale == true)
			{
				MaleCheckbox.isOn = true;
				FemaleCheckbox.isOn = false;
			}
			else
			{
				MaleCheckbox.isOn = false;
				FemaleCheckbox.isOn = true;
			}

			_skipNextCheck = false;
		}
		else
			Debug.LogWarning(string.Format("Could not set initial value: {0}", value));
	}

	private void Awake()
	{
		MaleCheckbox.onValueChanged.AddListener(MaleCheckboxValueChange);
		FemaleCheckbox.onValueChanged.AddListener(FemaleCheckboxValueChange);
	}

	private void MaleCheckboxValueChange(bool newValue) { ProcessValueChange(isMaleCheckBox: true, newValue: newValue); }
	private void FemaleCheckboxValueChange(bool newValue) { ProcessValueChange(isMaleCheckBox: false, newValue: newValue); }

	private void ProcessValueChange(bool isMaleCheckBox, bool newValue)
	{
		if(newValue/*== true*/ && !_skipNextCheck)
		{
			if (isMaleCheckBox)
				FemaleCheckbox.isOn = false;
			else
				MaleCheckbox.isOn = false;

			var result = isMaleCheckBox ? UserProfileGender.MALE : UserProfileGender.FEMALE;
			base.RaiseEntryEdited(result);
		}
	}
}
