using System.Collections;
using UnityEngine;

namespace Xsolla.Core
{
	internal static class CreateOrderCooldown
	{
		private const float CooldownDuration = 5f;

		public static bool IsActive => CooldownCoroutine != null;

		private static Coroutine CooldownCoroutine { get; set; }

		public static void Start()
		{
			if (!IsActive)
				CooldownCoroutine = CoroutinesExecutor.Run(CooldownRoutine());
		}

		public static void Cancel()
		{
			if (CooldownCoroutine == null)
				return;

			XDebug.Log("[CreateOrderCooldown] Cooldown cancelled manually before timeout");
			CoroutinesExecutor.Stop(CooldownCoroutine);
			CooldownCoroutine = null;
		}

		private static IEnumerator CooldownRoutine()
		{
			XDebug.Log($"[CreateOrderCooldown] Cooldown started for {CooldownDuration} seconds");
			yield return new WaitForSeconds(CooldownDuration);

			XDebug.Log("[CreateOrderCooldown] Cooldown ended automatically after timeout");
			CooldownCoroutine = null;
		}
	}
}