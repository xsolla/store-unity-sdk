using UnityEngine;

namespace Xsolla.Demo
{
	public class TimeService
	{
		private float TimeScale { get; set; } = 1f;

		public float DeltaTime => Time.deltaTime * TimeScale;

		public void Freeze()
		{
			TimeScale = 0f;
		}

		public void Unfreeze()
		{
			TimeScale = 1f;
		}
	}
}