using UnityEngine;

namespace Xsolla.TechDemo.Ui
{
	public class ScenariosGridWidget : MonoBehaviour
	{
		[field: SerializeField] private ScenarioWidget WidgetPrefab { get; set; }
		[field: SerializeField] private Transform WidgetsContainer { get; set; }

		private ServiceLocator ServiceLocator { get; set; }

		private void Awake()
		{
			WidgetsContainer.DeleteAllChildren();
		}

		public void Construct(ServiceLocator serviceLocator, ScenarioGroup group)
		{
			ServiceLocator = serviceLocator;
			Redraw(group);
		}

		public void Redraw(ScenarioGroup group)
		{
			WidgetsContainer.DeleteAllChildren();

			var scenarioTypes = ServiceLocator.ScenarioRegistry.GetScenarioTypes(group);
			foreach (var scenarioType in scenarioTypes)
			{
				var button = Instantiate(WidgetPrefab, WidgetsContainer, false);
				button.Construct(scenarioType, ServiceLocator);
			}
		}
	}
}