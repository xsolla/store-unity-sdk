using UnityEngine;

namespace Xsolla.TechDemo.Ui
{
	public class UiRoot : MonoBehaviour
	{
		[field: SerializeField] private ScenarioGroupsWidget ScenarioGroupsWidget { get; set; }
		[field: SerializeField] private ScenariosGridWidget ScenariosGridWidget { get; set; }
		[field: SerializeField] private ProcessWidget ProcessWidget { get; set; }
		[field: SerializeField] private PopupWidget SuccessPopupWidget { get; set; }
		[field: SerializeField] private PopupWidget ErrorPopupWidget { get; set; }

		public void Construct(ServiceLocator serviceLocator)
		{
			var selectedGroup = ScenarioGroup.Login;
			ScenarioGroupsWidget.Construct(serviceLocator, selectedGroup);
			ScenariosGridWidget.Construct(serviceLocator, selectedGroup);
		}

		public void SelectGroup(ScenarioGroup group)
		{
			ScenariosGridWidget.Redraw(group);
		}

		public void ShowProcessPopup(string message)
		{
			ProcessWidget.Open(message);
		}

		public void CloseProcessPopup()
		{
			ProcessWidget.Close();
		}

		public void ShowSuccessPopup(string title, string message)
		{
			SuccessPopupWidget.Open(title, message);
		}

		public void ShowErrorPopup(string title, string message)
		{
			ErrorPopupWidget.Open(title, message);
		}
	}
}