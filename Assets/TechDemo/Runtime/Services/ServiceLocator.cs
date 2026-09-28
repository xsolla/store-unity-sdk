using UnityEngine;
using Xsolla.TechDemo.Ui;

namespace Xsolla.TechDemo
{
	public class ServiceLocator
	{
		public readonly ScenarioLogger ScenarioLogger;
		public readonly ScenarioRunner ScenarioRunner;
		public readonly ScenarioFactory ScenarioFactory;
		public readonly ScenarioRegistry ScenarioRegistry;
		public readonly UiRoot UiRoot;

		public ServiceLocator(UiRoot uiRoot, MonoBehaviour monoBehaviour)
		{
			ScenarioLogger = new ScenarioLogger();
			ScenarioRunner = new ScenarioRunner(ScenarioLogger, uiRoot, monoBehaviour);
			ScenarioRegistry = new ScenarioRegistry();
			ScenarioFactory = new ScenarioFactory();
			UiRoot = uiRoot;
		}
	}
}