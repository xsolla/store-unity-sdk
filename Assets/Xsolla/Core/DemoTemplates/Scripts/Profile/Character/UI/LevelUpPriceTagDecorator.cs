using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpPriceTagDecorator : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] private Text SourceText;
#pragma warning restore 0649

	private Text _targetText;

	private void Awake()
	{
		SimpleButton.OnCursorEnter += CopyButtonColor;
		SimpleButton.OnCursorExit += CopyButtonColor;
	}

	private void OnDestroy()
	{
		SimpleButton.OnCursorEnter -= CopyButtonColor;
		SimpleButton.OnCursorExit -= CopyButtonColor;
	}

	private void CopyButtonColor()
	{
		//Initialization is done here because target text is instantiated dynamically quite a time after scene load
		if (_targetText == null)
			_targetText = GetComponentInChildren<Text>();

		if (/*now*/_targetText != null)
			_targetText.color = SourceText.color;
	}
}
