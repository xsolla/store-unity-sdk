using System.Collections;
using UnityEngine;
using Xsolla.Core;
using Xsolla.TechDemo.Scenarios;
using Xsolla.TechDemo.Ui;

namespace Xsolla.TechDemo
{
	public class ScenarioRunner
	{
		private readonly ScenarioLogger ScenarioLogger;
		private readonly UiRoot UiRoot;
		private readonly MonoBehaviour MonoBehaviour;
		private const string LogTag = "ScenarioRunner";

		public ScenarioRunner(ScenarioLogger scenarioLogger, UiRoot uiRoot, MonoBehaviour monoBehaviour)
		{
			ScenarioLogger = scenarioLogger;
			UiRoot = uiRoot;
			MonoBehaviour = monoBehaviour;
		}

		public void Run(Scenario scenario)
		{
			MonoBehaviour.StartCoroutine(RunCoroutine(scenario));
		}

		private IEnumerator RunCoroutine(Scenario scenario)
		{
			var scenarioName = GetScenarioName(scenario);
			ScenarioLogger.Message(LogTag, $"Running scenario: {scenarioName}");

			scenario.SetUp();
			UiRoot.ShowProcessPopup($"Running scenario: {scenarioName}");

			yield return scenario.Execute(
				() => {
					scenario.TearDown();
					OnScenarioSuccess(scenario);
				},
				error => {
					scenario.TearDown();
					OnScenarioError(scenario, error);
				});
		}

		private void OnScenarioSuccess(Scenario scenario)
		{
			var scenarioName = GetScenarioName(scenario);
			ScenarioLogger.Message(LogTag, $"Scenario '{scenarioName}' completed successfully");
			UiRoot.CloseProcessPopup();
			UiRoot.ShowSuccessPopup("Success", $"Scenario '{scenarioName}' completed successfully");
		}

		private void OnScenarioError(Scenario scenario, Error error)
		{
			var scenarioName = GetScenarioName(scenario);
			ScenarioLogger.Error(LogTag, $"Scenario '{scenarioName}' failed. Error: " + error.errorMessage);
			UiRoot.CloseProcessPopup();
			UiRoot.ShowErrorPopup("Error", $"Scenario '{scenarioName}' failed. Error: " + error.errorMessage);
		}

		private string GetScenarioName(Scenario scenario)
		{
			return scenario.GetType().Name;
		}
	}
}