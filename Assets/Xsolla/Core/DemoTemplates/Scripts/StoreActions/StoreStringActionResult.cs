using System;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public abstract class StoreStringActionResult : MonoBehaviour, IStoreStringAction
	{
		public Action<string> OnSuccess { get; set; }
		public Action<Error> OnError { get; set; }
	}
}
