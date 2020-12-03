using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class LevelUpPriceTagDecorator : MonoBehaviour
	{
		[SerializeField] private Text SourceText = default;

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
}
