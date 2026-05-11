using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Xsolla.TechDemo.Ui
{
	public class ScenarioGroupsWidget : MonoBehaviour
	{
		[field: SerializeField] private TMP_Dropdown GroupDropdown { get; set; }

		private ServiceLocator ServiceLocator { get; set; }

		public void Construct(ServiceLocator serviceLocator, ScenarioGroup group)
		{
			ServiceLocator = serviceLocator;

			GroupDropdown.onValueChanged.RemoveAllListeners();
			GroupDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

			RedrawDropdown(group);
		}

		private void RedrawDropdown(ScenarioGroup selectedGroup)
		{
			var allGroups = GetGroupsForDraw();

			var options = new TMP_Dropdown.OptionDataList();
			foreach (var group in allGroups)
				options.options.Add(new TMP_Dropdown.OptionData(group.ToString()));
			GroupDropdown.options = options.options;

			var selectedGroupIndex = allGroups.IndexOf(selectedGroup);
			GroupDropdown.SetValueWithoutNotify(selectedGroupIndex);
		}

		private void OnDropdownValueChanged(int index)
		{
			var groups = GetGroupsForDraw();
			var group = groups[index];
			ServiceLocator.UiRoot.SelectGroup(group);
		}

		private static List<ScenarioGroup> GetGroupsForDraw()
		{
			return new List<ScenarioGroup> {
				ScenarioGroup.Login,
				ScenarioGroup.Store,
				ScenarioGroup.Purchase
			};
		}
	}
}