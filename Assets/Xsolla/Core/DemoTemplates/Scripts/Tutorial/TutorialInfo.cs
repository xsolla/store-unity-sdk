using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialInfo", menuName = "Xsolla/ScriptableObjects/TutorialInfo", order = 1)]
public class TutorialInfo : ScriptableObject
{
	[Serializable]
	public class TutorialStep
	{
		public string title;
		public string description;
		public string prevButtonText;
		public string nextButtonText;
		public string highlightTag;
	}
	
	public List<TutorialStep> tutorialSteps;
}

