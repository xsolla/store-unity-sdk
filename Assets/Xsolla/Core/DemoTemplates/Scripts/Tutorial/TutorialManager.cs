using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;

public class TutorialManager : MonoBehaviour
{
	[SerializeField] private TutorialInfo _tutorialInfo;

#if UNITY_EDITOR
	[ContextMenu("Drop tutorial")]
	public void DropTutorial() => PlayerPrefs.SetInt(Constants.INVENTORY_TUTORIAL_COMPLETED, 0);
#endif

	public void ShowTutorial(bool showWelcomeMessage = true)
	{
		var tutorialPopup = PopupFactory.Instance.CreateTutorial();
		tutorialPopup.SetTutorialInfo(_tutorialInfo, showWelcomeMessage);
		tutorialPopup.SetCancelCallback(MarkTutorialAsCompleted);
		tutorialPopup.SetCompletionCallback(MarkTutorialAsCompleted);
	}

	public bool IsTutorialCompleted()
	{
		return PlayerPrefs.GetInt(Constants.INVENTORY_TUTORIAL_COMPLETED, 0) > 0;
	}

	void MarkTutorialAsCompleted()
	{
		PlayerPrefs.SetInt(Constants.INVENTORY_TUTORIAL_COMPLETED, 1);
	}
}