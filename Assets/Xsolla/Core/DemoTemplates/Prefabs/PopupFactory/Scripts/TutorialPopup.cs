using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Core.Popup
{
	public class TutorialPopup : MonoBehaviour, ITutorialPopup
	{
		[SerializeField] private SimpleTextButton BackButton;
		[SerializeField] private SimpleTextButton NextButton;
		[SerializeField] private SimpleTextButton StartButton;
		[SerializeField] private SimpleTextButton CancelButton;
		
		[SerializeField] private SimpleButton CloseButton;

		[SerializeField] private Text Title;
		[SerializeField] private Text Description;

		private TutorialInfo _tutorialInfo;
		private bool _showWelcomeMessage;

		private Action _onTutorialCompleted;

		private int _currentStepIndex;

		private bool IsTutorialInfoValid => _tutorialInfo != null && _tutorialInfo.tutorialSteps.Count > 0;
		private int FirstStepIndex => _showWelcomeMessage ? 0 : 1;

		private void Awake()
		{
			BackButton.onClick = MoveToPreviousStep;
			NextButton.onClick = MoveToNextStep;
			StartButton.onClick = MoveToNextStep;

			CancelButton.onClick = () => { Destroy(gameObject, 0.001F); };
			CloseButton.onClick = () => { Destroy(gameObject, 0.001F); };
		}

		public ITutorialPopup SetTutorialInfo(TutorialInfo info, bool showWelcomeMessage)
		{
			_tutorialInfo = info;
			_showWelcomeMessage = showWelcomeMessage;

			// skip welcome step if we're supposed to
			_currentStepIndex = FirstStepIndex;

			RefreshTutorial();

			return this;
		}

		void MoveToNextStep()
		{
			if (!IsTutorialInfoValid)
			{
				Destroy(gameObject, 0.001F);
				return;
			}

			_currentStepIndex++;

			if (_currentStepIndex >= _tutorialInfo.tutorialSteps.Count)
			{
				_onTutorialCompleted?.Invoke();
				Destroy(gameObject, 0.001F);
				return;
			}

			RefreshTutorial();
		}

		void MoveToPreviousStep()
		{
			if (!IsTutorialInfoValid)
			{
				Destroy(gameObject, 0.001F);
				return;
			}

			_currentStepIndex--;

			RefreshTutorial();
		}

		public ITutorialPopup SetCancelCallback(Action onCancel)
		{
			CancelButton.onClick = () =>
			{
				onCancel?.Invoke();
				Destroy(gameObject, 0.001F);
			};
			return this;
		}

		public ITutorialPopup SetCompletionCallback(Action onComplete)
		{
			_onTutorialCompleted = onComplete;
			return this;
		}

		void RefreshTutorial()
		{
			var currentStepInfo = _tutorialInfo.tutorialSteps[_currentStepIndex];

			Title.text = currentStepInfo.title.Replace("\\n", "\n");
			Description.text = currentStepInfo.description.Replace("\\n", "\n");

			NextButton.Text = currentStepInfo.nextButtonText;
			BackButton.Text = currentStepInfo.prevButtonText;
			StartButton.Text = currentStepInfo.nextButtonText;
			CancelButton.Text = currentStepInfo.prevButtonText;

			RefreshButtonsVisibility();
		}

		void RefreshButtonsVisibility()
		{
			if (_currentStepIndex == FirstStepIndex)
			{
				if (_showWelcomeMessage)
				{
					SetDefaultButtonsVisibility(false);
					SetWelcomeButtonsVisibility(true);
				}
				else
				{
					// no Back button should be displayed for first tutorial step
					BackButton.gameObject.SetActive(false);
				}
			}
			else
			{
				SetDefaultButtonsVisibility(true);
				SetWelcomeButtonsVisibility(false);
			}
		}

		void SetDefaultButtonsVisibility(bool show)
		{
			BackButton.gameObject.SetActive(show);
			NextButton.gameObject.SetActive(show);
		}

		void SetWelcomeButtonsVisibility(bool show)
		{
			StartButton.gameObject.SetActive(show);
			CancelButton.gameObject.SetActive(show);
		}
	}
}