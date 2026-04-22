using Xsolla.Core;
using Xsolla.TechDemo.Scenarios;
using Xsolla.TechDemo.Ui;

namespace Xsolla.TechDemo
{
	public class ScenarioRunner
	{
		private readonly ScenarioLogger ScenarioLogger;
		private readonly UiRoot UiRoot;
		private const string LogTag = "ScenarioRunner";

		public ScenarioRunner(ScenarioLogger scenarioLogger, UiRoot uiRoot)
		{
			ScenarioLogger = scenarioLogger;
			UiRoot = uiRoot;
		}

		public void Run(Scenario scenario)
		{
			var scenarioName = GetScenarioName(scenario);
			ScenarioLogger.Message(LogTag, $"Running scenario: {scenarioName}");

			scenario.Execute(
				() => OnScenarioSuccess(scenario),
				error => OnScenarioError(scenario, error));

			UiRoot.ShowProcessPopup($"Running scenario: {scenarioName}");
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