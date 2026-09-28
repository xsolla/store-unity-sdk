using System;
using Xsolla.TechDemo.Scenarios;

namespace Xsolla.TechDemo
{
	public class ScenarioFactory
	{
		public Scenario Create(Type scenarioType)
		{
			return (Scenario) Activator.CreateInstance(scenarioType);
		}
	}
}