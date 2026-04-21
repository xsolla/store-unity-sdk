using System;
using UnityEngine;

namespace Xsolla.Demo
{
	public class GuyMovementState : MonoBehaviour
	{
		public float InputHorizontal { get; set; }
		public Vector3 CurrentVelocity { get; set; }

		public event Action JumpBegan;
		public event Action JumpEnded;

		public void OnJumpBegan()
			=> JumpBegan?.Invoke();

		public void OnJumpEnded()
			=> JumpEnded?.Invoke();
	}
}