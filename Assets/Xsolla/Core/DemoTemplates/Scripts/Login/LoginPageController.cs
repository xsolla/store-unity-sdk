using System;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public abstract class LoginPageController : MonoBehaviour, IStoreActionResult, IStoreActionProgress
	{
		public Action OnStarted { get; set; }
		public Action OnSuccess { get; set; }
		public Action<Error> OnError { get; set; }
		public bool IsInProgress { get; protected set; }
	}
}
