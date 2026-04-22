using System;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.TechDemo.Scenarios;
using Xsolla.TechDemo.Ui;

namespace Xsolla.TechDemo
{
	public class ScenarioRegistry
	{
		private readonly Dictionary<ScenarioGroup, List<Type>> ScenarioTypes = new();

		public IReadOnlyCollection<Type> GetScenarioTypes(ScenarioGroup group)
		{
			return EnsureGroup(group);
		}

		public void RegisterScenarioType<T>(ScenarioGroup group) where T : Scenario
		{
			var list = EnsureGroup(group);
			var type = typeof(T);
			if (list.Contains(type))
			{
				Debug.LogError($"List already contains type: {type}");
				return;
			}

			list.Add(type);
		}

		private List<Type> EnsureGroup(ScenarioGroup group)
		{
			var list = ScenarioTypes.GetValueOrDefault(group);
			if (list == null)
			{
				list = new List<Type>();
				ScenarioTypes[group] = list;
			}

			return list;
		}
	}
}