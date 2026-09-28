using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.TechDemo.Ui
{
	public class ScenarioWidget : MonoBehaviour
	{
		[field: SerializeField] private TMP_Text NameText { get; set; }
		[field: SerializeField] private Button LaunchButton { get; set; }

		private Type ScenarioType { get; set; }
		private ServiceLocator ServiceLocator { get; set; }

		public void Construct(Type scenarioType, ServiceLocator serviceLocator)
		{
			ScenarioType = scenarioType;
			ServiceLocator = serviceLocator;
			NameText.text = scenarioType.Name.Nicify();
		}

		private void OnEnable()
		{
			LaunchButton.onClick.AddListener(LaunchScenario);
		}

		private void OnDisable()
		{
			LaunchButton.onClick.RemoveListener(LaunchScenario);
		}

		private void LaunchScenario()
		{
			var scenario = ServiceLocator.ScenarioFactory.Create(ScenarioType);
			ServiceLocator.ScenarioRunner.Run(scenario);
		}
	}
}