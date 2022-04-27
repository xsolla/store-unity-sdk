using System;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public abstract class LoginAuthorization : MonoBehaviour
	{
		public Action<string> OnSuccess { get; set; }
		public Action<Error> OnError { get; set; }

		public abstract void TryAuth(params object[] args);
	}
}
