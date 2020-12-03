using System;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public abstract class StoreActionResult : MonoBehaviour, IStoreActionResult
	{
		public Action OnSuccess { get; set; }
		public Action<Error> OnError { get; set; }
	}
}
