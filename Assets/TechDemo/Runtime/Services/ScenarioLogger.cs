using UnityEngine;

namespace Xsolla.TechDemo
{
	public class ScenarioLogger
	{
		public void Message(string tag, string message)
		{
			Debug.Log($"[{tag}] {message}");
		}

		public void Error(string tag, string message)
		{
			Debug.LogError($"[{tag}] {message}");
		}
	}
}